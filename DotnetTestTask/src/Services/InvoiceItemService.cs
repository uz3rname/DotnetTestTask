using DotnetTestTask.Data;
using DotnetTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Services
{
    public struct CreateInvoiceItemArguments
    {
        public int Amount { get; set; }
        public StoreItem StoreItem { get; set; }
    }

    public interface IInvoiceItemService : IBaseService<InvoiceItem, CreateInvoiceItemArguments>
    {}
    
    public class DbInvoiceItemService : BaseService<InvoiceItem, CreateInvoiceItemArguments>, IInvoiceItemService
    {
        public DbInvoiceItemService(AppDbContext dbContext) : base(dbContext)
        {
        }
        
        public override DbSet<InvoiceItem> GetEntities() => _dbContext.InvoiceItems;

        public override InvoiceItem Create(CreateInvoiceItemArguments args) => new InvoiceItem {
            Amount = args.Amount,
            Id = Guid.NewGuid(),
            StoreItem = args.StoreItem,
            StoreItemId = args.StoreItem.Id,
        };
    }
}