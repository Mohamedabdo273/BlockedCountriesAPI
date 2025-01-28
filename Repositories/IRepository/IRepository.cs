
using System.Linq.Expressions;

namespace BlockedCountriesAPI.Repositories.IRepository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null);           // Get all items
        T GetById(Expression<Func<T, bool>> expression);              // Get a single item by ID or key
        void Add(T entity);                // Add a new item
        void Update(T entity);             // Update an existing item
        void Delete(T entity);            // Delete an item by ID or key (take ID instead of entity)

    }
}

