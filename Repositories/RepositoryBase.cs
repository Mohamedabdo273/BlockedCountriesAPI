using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        protected List<T> _dataStore;

        public RepositoryBase()
        {
            _dataStore = new List<T>();
        }

        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null)
        {
            var query = _dataStore.AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return query.ToList();
        }

        public virtual T GetById(Expression<Func<T, bool>> expression)
        {
            var query = _dataStore.AsQueryable();
            return query.FirstOrDefault(expression);
        }

        public virtual void Add(T entity)
        {
            _dataStore.Add(entity);
        }

        public virtual void Update(T entity)
        {
            var existing = GetById(e => GetId(e).Equals(GetId(entity)));
            if (existing != null)
            {
                _dataStore.Remove(existing);
                _dataStore.Add(entity);
            }
        }

        public virtual void Delete(T entity)
        {
            _dataStore.Remove(entity);
        }

        protected abstract object GetId(T entity); // يجب أن يتم تنفيذها في الفئات الفرعية
    }
}
