using System.Linq.Expressions;

namespace BlockedCountriesAPI.Repositories.IRepository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null);
        T GetById(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
