using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Services.IServices
{
    public interface ICountryBlockService
    {
        bool BlockCountry(string countryCode); // Block a country.
        bool UnblockCountry(string countryCode); // Unblock a country.
        IEnumerable<BlockedCountry> GetBlockedCountries(int page, string searchTerm = null); // Get paginated and searchable blocked countries.
        void TemporarilyBlockCountry(TemporarilyBlockCountryRequest temporarilyBlockCountryRequest); // Temporarily block a country.
        bool IsCountryBlocked(string countryCode); // Check if a country is blocked.
    }
}

