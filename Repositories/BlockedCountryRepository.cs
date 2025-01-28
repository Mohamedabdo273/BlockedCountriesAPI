using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Linq;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.Repositories
{
    public class BlockedCountryRepository : RepositoryBase<BlockedCountry>, IBlockedCountryRepository
    {
        private readonly ConcurrentDictionary<string, BlockedCountry> _blockedCountries;

        public BlockedCountryRepository()
        {
            _blockedCountries = new ConcurrentDictionary<string, BlockedCountry>();
        }

        protected override object GetId(BlockedCountry entity)
        {
            return entity.CountryCode; // استخدام CountryCode كـ ID
        }

        // إضافة دولة محظورة
        public override void Add(BlockedCountry entity)
        {
            _blockedCountries[entity.CountryCode] = entity;
        }

        // تحديث بيانات دولة محظورة
        public override void Update(BlockedCountry entity)
        {
            _blockedCountries[entity.CountryCode] = entity;
        }

        // حذف دولة محظورة
        public override void Delete(BlockedCountry entity)
        {
            _blockedCountries.TryRemove(entity.CountryCode, out _);
        }

        // استرجاع جميع الدول المحظورة
        public override IEnumerable<BlockedCountry> GetAll(Expression<Func<BlockedCountry, bool>>? expression = null)
        {
            var query = _blockedCountries.Values.AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return query.ToList();
        }

        // الحصول على دولة معينة باستخدام CountryCode
        public BlockedCountry GetByCountryCode(string countryCode)
        {
            _blockedCountries.TryGetValue(countryCode, out var country);
            return country;
        }
    }
}
