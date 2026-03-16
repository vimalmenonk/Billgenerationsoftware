using GlowvitraBilling.Api.Dtos;

namespace GlowvitraBilling.Api.Services;

public class GstCalculationService : IGstCalculationService
{
    public GstBreakdown Calculate(IReadOnlyCollection<InvoiceItemRequest> items, bool intraStateSale)
    {
        var subtotal = Math.Round(items.Sum(i =>
        {
            var lineTotal = i.BasePrice * i.Quantity;
            var baseAmount = lineTotal / (1m + (i.GstPercent / 100m));
            return baseAmount;
        }), 2);

        var totalTax = Math.Round(items.Sum(i =>
        {
            var lineTotal = i.BasePrice * i.Quantity;
            var baseAmount = lineTotal / (1m + (i.GstPercent / 100m));
            return lineTotal - baseAmount;
        }), 2);

        var breakdown = new GstBreakdown
        {
            Subtotal = subtotal
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

        breakdown.RoundOff = 0m;
        breakdown.GrandTotal = Math.Round(gross, 2);

        return breakdown;
    }
}
