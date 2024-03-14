using DotnetTestTask.Data;
using DotnetTestTask.Exceptions;
using DotnetTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Services
{
    public struct CreateStoreItemArguments
    {
        public int Amount { get; set; }
        public string Name { get; set; }
    }

    public interface IStoreItemService : IBaseService<StoreItem, CreateStoreItemArguments>
    {
        public Task<StoreItem?> GetByName(string Name);
    }

    public class DbStoreItemService : BaseService<StoreItem, CreateStoreItemArguments>, IStoreItemService
    {
        public DbStoreItemService(AppDbContext dbContext) : base(dbContext)
        {
        }
        
        public override DbSet<StoreItem> GetEntities()
        {
            return _dbContext.StoreItems;
        }

        public async Task<StoreItem?> GetByName(string Name) => await _dbContext.StoreItems
            .Where(e => e.Name == Name)
            .FirstOrDefaultAsync();

        public override StoreItem Create(CreateStoreItemArguments args) => new StoreItem
        {
            Amount = args.Amount,
            Id = Guid.NewGuid(),
            Name = args.Name,
        };

        public override async Task<StoreItem> Save(StoreItem entity)
        {
            if (entity.Amount < 0)
            {
                throw new InvalidAmount($"Item amount should be >= 0");
            }
            return await base.Save(entity);
        }

        public override async Task Update(StoreItem item)
        {
            if (item.Amount < 0)
            {
                throw new InvalidAmount($"Item amount should be >= 0");
            }
            await base.Update(item);
        }
    }
}
