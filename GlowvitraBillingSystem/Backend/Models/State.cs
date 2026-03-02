namespace GlowvitraBilling.Api.Models;

public class State
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<SellerProfile> SellerProfiles { get; set; } = new List<SellerProfile>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
