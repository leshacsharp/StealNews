using MongoDB.Driver;
using StealNews.Model.Entities;

namespace StealNews.DataProvider.Context
{
    public interface IDbContext
    {
        IMongoCollection<News> News { get; }
        IMongoCollection<TEntity> Set<TEntity>() where TEntity : IMongoEntity;
    }
}
