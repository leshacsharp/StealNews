using MongoDB.Driver;
using StealNews.Model.Entities;
using System;

namespace StealNews.DataProvider.Context
{
    public class NewsContext : IDbContext
    {
        private readonly IMongoDatabase _db;

        public NewsContext(string connectionString)
        {
            if(connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var databaseName = new MongoUrl(connectionString).DatabaseName;
            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(databaseName);
        }

        public IMongoCollection<News> News
        {
            get
            {
                return _db.GetCollection<News>(nameof(News));
            }
        }

        public IMongoCollection<TEntity> Set<TEntity>() where TEntity : IMongoEntity
        {
            return _db.GetCollection<TEntity>(nameof(TEntity));
        }
    }
}
