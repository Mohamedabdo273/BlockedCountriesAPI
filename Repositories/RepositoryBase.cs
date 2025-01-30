using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        protected readonly List<T> _dataStore = new List<T>();

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null)
        {
            var query = _dataStore.AsQueryable();
            return expression != null ? query.Where(expression).ToList() : query.ToList();
        }

        public T GetById(Expression<Func<T, bool>> expression)
        {
            return _dataStore.AsQueryable().FirstOrDefault(expression)
                   ?? throw new KeyNotFoundException("Entity not found.");
        }


        public void Add(T entity)
        {
            if (_dataStore.Any(e => GetId(e).Equals(GetId(entity))))
                throw new InvalidOperationException("Entity with the same ID already exists.");

            _dataStore.Add(entity);
        }

        public void Update(T entity)
        {
            var existing = _dataStore.FirstOrDefault(e => GetId(e).Equals(GetId(entity)));
            if (existing == null)
                throw new KeyNotFoundException("Entity not found for update.");

            _dataStore.Remove(existing);
            _dataStore.Add(entity);
        }

        public void Delete(T entity)
        {
            if (!_dataStore.Remove(entity))
                throw new KeyNotFoundException("Entity not found for deletion.");
        }

        protected abstract object GetId(T entity);
    }
}
