using MongoDB.Bson;

namespace StealNews.Model.Entities
{
    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
    }
}
