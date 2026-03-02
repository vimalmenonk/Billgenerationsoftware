namespace GlowvitraBilling.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HsnCode { get; set; } = string.Empty;
    public decimal GstPercent { get; set; }
}
