using DotnetTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}

        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceCreator> InvoiceCreators { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.HasPostgresExtension("uuid-ossp");
            
            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.InvoiceItems)
                .WithOne(e => e.Invoice)
                .HasForeignKey(e => e.InvoiceId)
                .IsRequired();

            modelBuilder.Entity<Invoice>()
                .HasOne(e => e.InvoiceCreator)
                .WithMany(e => e.Invoices)
                .HasForeignKey(e => e.InvoiceCreatorId)
                .IsRequired();

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(e => e.StoreItem)
                .WithMany(e => e.InvoiceItems)
                .HasForeignKey(e => e.StoreItemId)
                .IsRequired();
        }
    }
}