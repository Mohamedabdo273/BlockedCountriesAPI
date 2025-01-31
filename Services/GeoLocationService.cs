using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services.IServices;
using Newtonsoft.Json;

namespace BlockedCountriesAPI.Services
{
    public class GeoLocationService : IGeoLocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeoLocationService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public GeoLocationService(HttpClient httpClient, IConfiguration configuration, ILogger<GeoLocationService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiKey = configuration["ThirdPartyAPI:ApiKey"] ?? throw new ArgumentNullException("API Key is missing.");
            _baseUrl = configuration["ThirdPartyAPI:BaseUrl"] ?? throw new ArgumentNullException("Base URL is missing.");
        }

        public async Task<GeoLocationResponse> GetGeoLocationAsync(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP address cannot be null or empty.");

            try
            {
                var url = $"{_baseUrl}?apiKey={_apiKey}&ip={ipAddress}";

                _logger.LogInformation($"Fetching geolocation for {ipAddress}...");

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error fetching geolocation details for {ipAddress}: {errorContent}");
                    throw new HttpRequestException($"Error fetching geolocation details: {errorContent}");
                }

                var content = await response.Content.ReadAsStringAsync();

                // Deserialize the response into a dynamic object or a custom class
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(content);

                // Map the response fields to GeoLocationResponse
                var geoLocationResponse = new GeoLocationResponse
                {
                    IP = ipAddress,
                    CountryCode =  apiResponse.country_code2,                 
                    CountryName =  apiResponse.country_name,
                    ISP = apiResponse.isp
                };

                _logger.LogInformation($"Geolocation for {ipAddress} fetched successfully.");
                return geoLocationResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching geolocation details.");
                throw;
            }
        }

    }
}
