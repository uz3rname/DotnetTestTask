using DotnetTestTask.Data;
using DotnetTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Services
{
    public struct CreateInvoiceCreatorArguments
    {
        public string Name { get; set; }
    }

    public interface IInvoiceCreatorService : IBaseService<InvoiceCreator, CreateInvoiceCreatorArguments>
    {
        public Task<InvoiceCreator> GetByNameOrCreate(string name);
    }
    
    public class DbInvoiceCreatorService : IInvoiceCreatorService
    {
        private readonly AppDbContext _dbContext;

        public DbInvoiceCreatorService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<InvoiceCreator>> GetAll() => await _dbContext.InvoiceCreators.ToListAsync();

        public async Task<InvoiceCreator?> GetById(Guid id) => await _dbContext.InvoiceCreators.FindAsync(id);
        
        public async Task<InvoiceCreator> GetByNameOrCreate(string name)
        {
            var creator = await _dbContext.InvoiceCreators
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();
            
            if (creator != null)
            {
                return creator;
            }
            else
            {
                return await Save(new CreateInvoiceCreatorArguments {
                    Name = name,
                });
            }
        }

        public InvoiceCreator Create(CreateInvoiceCreatorArguments args) => new InvoiceCreator {
            Id = Guid.NewGuid(),
            Name = args.Name,
        };

        public async Task<InvoiceCreator> Save(InvoiceCreator entity)
        {
            await _dbContext.InvoiceCreators.AddAsync(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                _dbContext.InvoiceCreators.Remove(entity);
                throw;
            }
            return entity;
        }

        public async Task<InvoiceCreator> Save(CreateInvoiceCreatorArguments args) => await Save(Create(args));

        public async Task Delete(InvoiceCreator item)
        {
            _dbContext.InvoiceCreators.Remove(item);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                await _dbContext.InvoiceCreators.AddAsync(item);
                throw;
            }
        }
        
        public async Task Update(InvoiceCreator item)
        {
            _dbContext.InvoiceCreators.Update(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}