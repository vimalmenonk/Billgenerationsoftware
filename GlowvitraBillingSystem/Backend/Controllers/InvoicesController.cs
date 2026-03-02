using GlowvitraBilling.Api.Dtos;
using GlowvitraBilling.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlowvitraBilling.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost("summary")]
    public async Task<IActionResult> GetSummary([FromBody] InvoiceCreateRequest request)
    {
        var summary = await _invoiceService.GetDraftSummaryAsync(request.CustomerStateId, request.Items);
        return Ok(summary);
    }

    [HttpPost("generate-pdf")]
    public async Task<IActionResult> GeneratePdf([FromBody] InvoiceCreateRequest request)
    {
        var result = await _invoiceService.GenerateInvoicePdfAsync(request);
        return File(result.PdfBytes, "application/pdf", $"{result.InvoiceNumber}.pdf");
    }
}
