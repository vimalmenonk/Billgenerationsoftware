using GlowvitraBilling.Api.Data;
using GlowvitraBilling.Api.Documents;
using GlowvitraBilling.Api.Dtos;
using GlowvitraBilling.Api.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace GlowvitraBilling.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly BillingDbContext _dbContext;
    private readonly IGstCalculationService _gstService;

    public InvoiceService(BillingDbContext dbContext, IGstCalculationService gstService)
    {
        _dbContext = dbContext;
        _gstService = gstService;
    }

    public async Task<InvoiceSummaryResponse> GetDraftSummaryAsync(int customerStateId, IReadOnlyCollection<InvoiceItemRequest> items)
    {
        var seller = await GetSellerAsync();
        var invoiceNumber = await GenerateInvoiceNumberAsync();
        var intraState = seller.StateId == customerStateId;
        var gst = _gstService.Calculate(items, intraState);

        return new InvoiceSummaryResponse
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.Now,
            Seller = MapSeller(seller),
            Subtotal = gst.Subtotal,
            CgstAmount = gst.CgstAmount,
            SgstAmount = gst.SgstAmount,
            IgstAmount = gst.IgstAmount,
            RoundOff = gst.RoundOff,
            GrandTotal = gst.GrandTotal
        };
    }

    public async Task<(byte[] PdfBytes, string InvoiceNumber)> GenerateInvoicePdfAsync(InvoiceCreateRequest request)
    {
        var seller = await GetSellerAsync();
        var invoiceNumber = await GenerateInvoiceNumberAsync();
        var intraState = seller.StateId == request.CustomerStateId;
        var gst = _gstService.Calculate(request.Items, intraState);

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = DateTime.Now,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerAddress = request.CustomerAddress,
            CustomerStateId = request.CustomerStateId,
            Pincode = request.Pincode,
            Subtotal = gst.Subtotal,
            CgstAmount = gst.CgstAmount,
            SgstAmount = gst.SgstAmount,
            IgstAmount = gst.IgstAmount,
            RoundOff = gst.RoundOff,
            GrandTotal = gst.GrandTotal,
            Items = request.Items.Select(i => new InvoiceItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                HsnCode = i.HsnCode,
                Quantity = i.Quantity,
                BasePrice = i.BasePrice,
                TaxAmount = Math.Round((i.BasePrice * i.Quantity) * (i.GstPercent / 100m), 2),
                LineTotal = Math.Round((i.BasePrice * i.Quantity) + ((i.BasePrice * i.Quantity) * (i.GstPercent / 100m)), 2)
            }).ToList()
        };

        _dbContext.Invoices.Add(invoice);
        await _dbContext.SaveChangesAsync();

        var stateName = await _dbContext.States
            .Where(s => s.Id == request.CustomerStateId)
            .Select(s => s.Name)
            .FirstAsync();

        var pdfBytes = new InvoicePdfDocument(invoice, seller, stateName, intraState).GeneratePdf();
        return (pdfBytes, invoiceNumber);
    }

    private async Task<SellerProfile> GetSellerAsync() =>
        await _dbContext.SellerProfiles.Include(s => s.State).FirstAsync();

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        var year = DateTime.Now.Year;
        var prefix = $"GV-{year}-";
        var count = await _dbContext.Invoices.CountAsync(i => i.InvoiceNumber.StartsWith(prefix));
        return $"{prefix}{(count + 1):D4}";
    }

    private static SellerResponse MapSeller(SellerProfile seller) => new()
    {
        Id = seller.Id,
        SellerName = seller.SellerName,
        Address = seller.Address,
        Gstin = seller.Gstin,
        StateId = seller.StateId,
        StateName = seller.State?.Name ?? string.Empty
    };
}
