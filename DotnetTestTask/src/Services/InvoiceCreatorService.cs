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
    
    public class DbInvoiceCreatorService : BaseService<InvoiceCreator, CreateInvoiceCreatorArguments>, IInvoiceCreatorService
    {
        public DbInvoiceCreatorService(AppDbContext dbContext) : base(dbContext)
        {
        }
        
        public override DbSet<InvoiceCreator> GetEntities() => _dbContext.InvoiceCreators;

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

        public override InvoiceCreator Create(CreateInvoiceCreatorArguments args) => new InvoiceCreator {
            Id = Guid.NewGuid(),
            Name = args.Name,
        };
    }
}