using DotnetTestTask.Data;
using DotnetTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Services
{
    public struct CreateInvoiceArguments
    {
        public InvoiceCreator InvoiceCreator { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; set; }
    }

    public interface IInvoiceService : IBaseService<Invoice, CreateInvoiceArguments>
    {}
    
    public class DbInvoiceService : IInvoiceService
    {
        private readonly AppDbContext _dbContext;

        public DbInvoiceService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Invoice>> GetAll() => await _dbContext.Invoices
            .Include(e => e.InvoiceCreator)
            .ToListAsync();
        
        public async Task<Invoice?> GetById(Guid id) => await _dbContext.Invoices
            .Include(e => e.InvoiceCreator)
            .Include(e => e.InvoiceItems)
            .ThenInclude(e => e.StoreItem)
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
        
        public Invoice Create(CreateInvoiceArguments args) => new Invoice {
            Id = Guid.NewGuid(),
            InvoiceCreator = args.InvoiceCreator,
            InvoiceCreatorId = args.InvoiceCreator.Id,
            InvoiceItems = args.InvoiceItems,
            Timestamp = DateTime.UtcNow,
        };
        
        public async Task<Invoice> Save(Invoice entity)
        {
            await _dbContext.Invoices.AddAsync(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (DbUpdateException ex)
            {
                _dbContext.Invoices.Remove(entity);
                if (ex.InnerException is Npgsql.PostgresException)
                {
                    var pgEx = (Npgsql.PostgresException)ex.InnerException;
                    if (pgEx.SqlState == "P0001")
                    {
                        throw new Exceptions.InvalidAmount(pgEx.MessageText);
                    }
                }
                throw;
            }
            catch
            {
                _dbContext.Invoices.Remove(entity);
                throw;
            }
        }
        
        public async Task<Invoice> Save(CreateInvoiceArguments args) => await Save(Create(args));
        
        public async Task Delete(Invoice item)
        {
            _dbContext.Invoices.Remove(item);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                await _dbContext.Invoices.AddAsync(item);
                throw;
            }
        }
        
        public async Task Update(Invoice item)
        {
            _dbContext.Invoices.Update(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}