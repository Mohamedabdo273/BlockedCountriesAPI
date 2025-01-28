using BlockedCountriesAPI.BackgroundServices;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly CountryBlockService _countryBlockService;

        // In-memory store for temporarily blocked countries
        private static ConcurrentDictionary<string, TemporalBlock> temporalBlocks = new ConcurrentDictionary<string, TemporalBlock>();

        // Inject the CountryBlockService in the constructor
        public CountryController(CountryBlockService countryBlockService)
        {
            _countryBlockService = countryBlockService;
        }

        // Endpoint to block a country
        [HttpPost("block")]
        public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
                return BadRequest("Country code cannot be null or empty.");

            try
            {
                _countryBlockService.BlockCountry(request.CountryCode);  // No need for result here
                return Ok($"Country {request.CountryCode} has been blocked.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to unblock a country
        [HttpPost("unblock")]
        public IActionResult UnblockCountry([FromBody] BlockCountryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
                return BadRequest("Country code cannot be null or empty.");

            try
            {
                bool result = _countryBlockService.UnblockCountry(request.CountryCode);
                return Ok($"Country {request.CountryCode} has been unblocked.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to get blocked countries with pagination
        [HttpGet("blockedcountries")]
        public IActionResult GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null)
        {
            try
            {
                var countries = _countryBlockService.GetBlockedCountries(page, pageSize, searchTerm);
                return Ok(countries);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to temporarily block a country
        [HttpPost("temporal-block")]
        public IActionResult TemporarilyBlockCountry([FromBody] TemporarilyBlockCountryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
                return BadRequest("Country code cannot be null or empty.");

            // Validate duration range
            if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
                return BadRequest("Duration must be between 1 and 1440 minutes.");

            // Check for invalid country codes (e.g., "XX")
            if (request.CountryCode.Length != 2)
                return BadRequest("Invalid country code.");

            // Prevent duplicate temporal blocks for the same country
            if (temporalBlocks.ContainsKey(request.CountryCode))
                return Conflict($"Country {request.CountryCode} is already temporarily blocked.");

            // Add country to the in-memory store with expiration time
            var expiryTime = DateTime.UtcNow.AddMinutes(request.DurationMinutes);
            temporalBlocks[request.CountryCode] = new TemporalBlock
            {
                CountryCode = request.CountryCode,
                ExpiryTime = expiryTime
            };

            return Ok($"Country {request.CountryCode} has been temporarily blocked for {request.DurationMinutes} minutes.");
        }

        // Endpoint to check if a country is blocked
        [HttpGet("isblocked/{countryCode}")]
        public IActionResult IsCountryBlocked(string countryCode)
        {
            try
            {
                bool isBlocked = _countryBlockService.IsCountryBlocked(countryCode);
                return Ok(isBlocked ? $"Country {countryCode} is blocked." : $"Country {countryCode} is not blocked.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
