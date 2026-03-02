using GlowvitraBilling.Api.Dtos;

namespace GlowvitraBilling.Api.Services;

public interface IInvoiceService
{
    Task<InvoiceSummaryResponse> GetDraftSummaryAsync(int customerStateId, IReadOnlyCollection<InvoiceItemRequest> items);
    Task<(byte[] PdfBytes, string InvoiceNumber)> GenerateInvoicePdfAsync(InvoiceCreateRequest request);
}
