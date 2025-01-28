using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Services.IServices
{
    public interface IGeoLocationService
    {
        Task<GeoLocationResponse> GetGeoLocationAsync(string ipAddress); // Fetch geolocation details for an IP.
    }

}
