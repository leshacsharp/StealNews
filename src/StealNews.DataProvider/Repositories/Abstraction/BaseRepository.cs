using MongoDB.Driver;
using StealNews.DataProvider.Context;
using StealNews.Model.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StealNews.DataProvider.Repositories.Abstraction
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : IMongoEntity
    {     
        protected IDbContext DB { get; set; }
        protected IMongoCollection<TEntity> Collection { get; set; }

        public BaseRepository(string connectionString)
        {
            DB = new NewsContext(connectionString);
            Collection = DB.Set<TEntity>();
        }    

        public async Task AddAsync(TEntity entity)
        {
            await Collection.InsertOneAsync(entity);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            var deleteFilter = Builders<TEntity>.Filter.Eq(p => p.Id, entity.Id);
            await Collection.DeleteOneAsync(deleteFilter);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var updateFilter = Builders<TEntity>.Filter.Eq(p => p.Id, entity.Id);
            await Collection.ReplaceOneAsync(updateFilter, entity);
        }

        public IQueryable<TEntity> Read(Expression<Func<TEntity, bool>> predicate)
        {
            return Collection.AsQueryable().Where(predicate);
        }
    }
}
