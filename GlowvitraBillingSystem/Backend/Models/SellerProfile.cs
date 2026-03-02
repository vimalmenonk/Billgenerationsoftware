namespace GlowvitraBilling.Api.Models;

public class SellerProfile
{
    public int Id { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Gstin { get; set; } = string.Empty;
    public int StateId { get; set; }
    public State? State { get; set; }
}
