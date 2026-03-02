using GlowvitraBilling.Api.Dtos;

namespace GlowvitraBilling.Api.Services;

public class GstCalculationService : IGstCalculationService
{
    public GstBreakdown Calculate(IReadOnlyCollection<InvoiceItemRequest> items, bool intraStateSale)
    {
        var subtotal = items.Sum(i => i.BasePrice * i.Quantity);
        var totalTax = items.Sum(i => (i.BasePrice * i.Quantity) * (i.GstPercent / 100m));

        var breakdown = new GstBreakdown
        {
            Subtotal = Math.Round(subtotal, 2)
        };

        if (intraStateSale)
        {
            breakdown.CgstAmount = Math.Round(totalTax / 2m, 2);
            breakdown.SgstAmount = Math.Round(totalTax / 2m, 2);
            breakdown.IgstAmount = 0m;
        }
        else
        {
            breakdown.CgstAmount = 0m;
            breakdown.SgstAmount = 0m;
            breakdown.IgstAmount = Math.Round(totalTax, 2);
        }

        var gross = breakdown.Subtotal + breakdown.CgstAmount + breakdown.SgstAmount + breakdown.IgstAmount;
        var rounded = Math.Round(gross, 0, MidpointRounding.AwayFromZero);

        breakdown.RoundOff = Math.Round(rounded - gross, 2);
        breakdown.GrandTotal = Math.Round(gross + breakdown.RoundOff, 2);

        return breakdown;
    }
}
