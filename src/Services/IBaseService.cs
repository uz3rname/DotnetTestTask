using DotnetTestTask.Models;

namespace DotnetTestTask.Services
{
    public interface IBaseService<TEntity, TCreate> where TEntity : BaseEntity
    {
        public Task<List<TEntity>> GetAll();
        public Task<TEntity?> GetById(Guid id);
        public TEntity Create(TCreate args);
        public Task<TEntity> Save(TEntity item);
        public Task<TEntity> Save(TCreate args);
        public Task Delete(TEntity item);
        public Task Update(TEntity entity);
    }
}