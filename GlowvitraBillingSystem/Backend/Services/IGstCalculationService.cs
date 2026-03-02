using GlowvitraBilling.Api.Dtos;

namespace GlowvitraBilling.Api.Services;

public interface IGstCalculationService
{
    GstBreakdown Calculate(IReadOnlyCollection<InvoiceItemRequest> items, bool intraStateSale);
}

public class GstBreakdown
{
    public decimal Subtotal { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal RoundOff { get; set; }
    public decimal GrandTotal { get; set; }
}
