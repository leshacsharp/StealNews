using StealNews.Model.Entities;
using System.Threading.Tasks;

namespace StealNews.DataProvider.Repositories.Abstraction
{
    public interface IBaseRepository<TEntity> where TEntity : IMongoEntity
    {
        Task AddAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task UpdateAsync(TEntity entity); 
    }
}
