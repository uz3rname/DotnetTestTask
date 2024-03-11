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

    public class DbStoreItemService : IStoreItemService
    {
        private readonly AppDbContext _dbContext;

        public DbStoreItemService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<StoreItem>> GetAll() => await _dbContext.StoreItems.ToListAsync();

        public async Task<StoreItem?> GetById(Guid id) => await _dbContext.StoreItems.FindAsync(id);

        public async Task<StoreItem?> GetByName(string Name) => await _dbContext.StoreItems
            .Where(e => e.Name == Name)
            .FirstOrDefaultAsync();

        public StoreItem Create(CreateStoreItemArguments args) => new StoreItem
        {
            Amount = args.Amount,
            Id = Guid.NewGuid(),
            Name = args.Name,
        };

        public async Task<StoreItem> Save(StoreItem entity)
        {
            if (entity.Amount < 0)
            {
                throw new InvalidAmount($"Item amount should be >= 0");
            }
            await _dbContext.StoreItems.AddAsync(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                _dbContext.StoreItems.Remove(entity);
                throw;
            }
            return entity;
        }

        public async Task<StoreItem> Save(CreateStoreItemArguments args) => await Save(Create(args));

        public async Task Delete(StoreItem item)
        {
            _dbContext.StoreItems.Remove(item);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                await _dbContext.StoreItems.AddAsync(item);
                throw;
            }
        }

        public async Task Update(StoreItem item)
        {
            if (item.Amount < 0)
            {
                throw new InvalidAmount($"Item amount should be >= 0");
            }
            _dbContext.StoreItems.Update(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}
