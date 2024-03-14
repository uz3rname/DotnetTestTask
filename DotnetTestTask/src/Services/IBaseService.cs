using DotnetTestTask.Data;
using DotnetTestTask.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Services
{
    public interface IBaseService<TEntity, TCreate> where TEntity : BaseEntity
    {
        public Task<List<TEntity>> GetAll();
        public Task<TEntity?> GetById(Guid id);
        public abstract TEntity Create(TCreate args);
        public Task<TEntity> Save(TEntity item);
        public Task<TEntity> Save(TCreate args);
        public Task Delete(TEntity item);
        public Task Update(TEntity entity);
        public abstract DbSet<TEntity> GetEntities();
    }
    
    public abstract class BaseService<TEntity, TCreate> : IBaseService<TEntity, TCreate> where TEntity : BaseEntity
    {
        protected readonly AppDbContext _dbContext;

        public BaseService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<List<TEntity>> GetAll() => await GetEntities().ToListAsync();

        public virtual async Task<TEntity?> GetById(Guid id) => await GetEntities().FindAsync(id);
        
        public abstract TEntity Create(TCreate args);

        public virtual async Task<TEntity> Save(TEntity entity)
        {
            await GetEntities().AddAsync(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                GetEntities().Remove(entity);
                throw;
            }
            return entity;
        }

        public virtual async Task<TEntity> Save(TCreate args) => await Save(Create(args));

        public virtual async Task Delete(TEntity entity)
        {
            GetEntities().Remove(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                await GetEntities().AddAsync(entity);
                throw;
            }
        }
        
        public virtual async Task Update(TEntity entity)
        {
            GetEntities().Update(entity);
            await _dbContext.SaveChangesAsync();
        }
        
        public abstract DbSet<TEntity> GetEntities();
    }
}