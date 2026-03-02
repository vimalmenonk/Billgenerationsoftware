namespace GlowvitraBilling.Api.Dtos;

public class InvoiceCreateRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public int CustomerStateId { get; set; }
    public string Pincode { get; set; } = string.Empty;
    public List<InvoiceItemRequest> Items { get; set; } = new();
}

public class InvoiceItemRequest
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string HsnCode { get; set; } = string.Empty;
    public decimal GstPercent { get; set; }
    public int Quantity { get; set; }
    public decimal BasePrice { get; set; }
}

public class InvoiceSummaryResponse
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public SellerResponse Seller { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal RoundOff { get; set; }
    public decimal GrandTotal { get; set; }
}

public class SellerResponse
{
    public int Id { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Gstin { get; set; } = string.Empty;
    public int StateId { get; set; }
    public string StateName { get; set; } = string.Empty;
}
