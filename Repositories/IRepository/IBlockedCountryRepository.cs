using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Repositories.IRepository
{
    public interface IBlockedCountryRepository : IRepository<BlockedCountry>
    {
        BlockedCountry GetByCountryCode(string countryCode); // Additional specific method
    }

}
