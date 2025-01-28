using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services.IServices;
using Newtonsoft.Json;

namespace BlockedCountriesAPI.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public GeoLocationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = configuration["ThirdPartyAPI:ApiKey"] ?? throw new ArgumentNullException("API Key is missing.");
            _baseUrl = configuration["ThirdPartyAPI:BaseUrl"] ?? throw new ArgumentNullException("Base URL is missing.");
        }

        public async Task<GeoLocationResponse> GetGeoLocationAsync(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP address cannot be null or empty.");

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}{ipAddress}/json/?key={_apiKey}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GeoLocationResponse>(content);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error fetching geolocation details.", ex);
            }
        }
    }
}
