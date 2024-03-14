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
    
    public class DbInvoiceService : BaseService<Invoice, CreateInvoiceArguments>, IInvoiceService
    {
        public DbInvoiceService(AppDbContext dbContext) : base(dbContext)
        {
        }
        
        public override DbSet<Invoice> GetEntities()
        {
            return _dbContext.Invoices;
        }

        public override async Task<List<Invoice>> GetAll() => await _dbContext.Invoices
            .Include(e => e.InvoiceCreator)
            .ToListAsync();
        
        public override async Task<Invoice?> GetById(Guid id) => await _dbContext.Invoices
            .Include(e => e.InvoiceCreator)
            .Include(e => e.InvoiceItems)
            .ThenInclude(e => e.StoreItem)
            .Where(e => e.Id == id)
            .FirstOrDefaultAsync();
        
        public override Invoice Create(CreateInvoiceArguments args) => new Invoice {
            Id = Guid.NewGuid(),
            InvoiceCreator = args.InvoiceCreator,
            InvoiceCreatorId = args.InvoiceCreator.Id,
            InvoiceItems = args.InvoiceItems,
            Timestamp = DateTime.UtcNow,
        };
        
        public override async Task<Invoice> Save(Invoice entity)
        {
            try
            {
                return await base.Save(entity);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is Npgsql.PostgresException)
                {
                    var pgEx = (Npgsql.PostgresException)ex.InnerException;
                    if (pgEx.SqlState == "P0001")
                    {
                        throw new Exceptions.InvalidAmount(pgEx.MessageText, ex);
                    }
                }
                throw;
            }
        }
    }
}