using GlowvitraBilling.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GlowvitraBilling.Api.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<State> States => Set<State>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SellerProfile>()
            .HasOne(s => s.State)
            .WithMany(s => s.SellerProfiles)
            .HasForeignKey(s => s.StateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.CustomerState)
            .WithMany(s => s.Invoices)
            .HasForeignKey(i => i.CustomerStateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InvoiceItem>()
            .HasOne(i => i.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(i => i.InvoiceId);

        modelBuilder.Entity<InvoiceItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
