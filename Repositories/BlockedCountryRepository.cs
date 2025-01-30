using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.Repositories
{
    public class BlockedCountryRepository : RepositoryBase<BlockedCountry>, IBlockedCountryRepository
    {
        
        protected override object GetId(BlockedCountry entity) => entity.CountryCode;

        public BlockedCountry GetByCountryCode(string countryCode)
        {
            var country = _dataStore.FirstOrDefault(c => c.CountryCode == countryCode);
            return country;
        }



    }
}
