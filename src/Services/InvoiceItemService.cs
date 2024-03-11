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
    
    public class DbInvoiceItemService : IInvoiceItemService
    {
        private readonly AppDbContext _dbContext;

        public DbInvoiceItemService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<InvoiceItem>> GetAll() => await _dbContext.InvoiceItems.ToListAsync();

        public async Task<InvoiceItem?> GetById(Guid id) => await _dbContext.InvoiceItems.FindAsync(id);

        public InvoiceItem Create(CreateInvoiceItemArguments args) => new InvoiceItem {
            Amount = args.Amount,
            Id = Guid.NewGuid(),
            StoreItem = args.StoreItem,
            StoreItemId = args.StoreItem.Id,
        };

        public async Task<InvoiceItem> Save(InvoiceItem entity)
        {
            await _dbContext.InvoiceItems.AddAsync(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                _dbContext.InvoiceItems.Remove(entity);
                throw;
            }
            return entity;
        }

        public async Task<InvoiceItem> Save(CreateInvoiceItemArguments args) => await Save(Create(args));

        public async Task Delete(InvoiceItem item)
        {
            _dbContext.InvoiceItems.Remove(item);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                await _dbContext.InvoiceItems.AddAsync(item);
                throw;
            }
        }
        
        public async Task Update(InvoiceItem item)
        {
            _dbContext.InvoiceItems.Update(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}