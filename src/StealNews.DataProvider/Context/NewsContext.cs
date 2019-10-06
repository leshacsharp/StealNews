using MongoDB.Driver;
using StealNews.Model.Entities;
using System;
using System.Security.Authentication;

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

            var mongoUrl = new MongoUrl(connectionString);
            MongoClientSettings settings = MongoClientSettings.FromUrl(mongoUrl);  
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 }; 

            var client = new MongoClient(settings);
            _db = client.GetDatabase(mongoUrl.DatabaseName);
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
            return _db.GetCollection<TEntity>(typeof(TEntity).Name);
        }
    }
}
