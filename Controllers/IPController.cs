using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPController : ControllerBase
    {
        private readonly GeoLocationService _geoLocationService;
        private readonly LogService _logService;

        // Inject services in the constructor
        public IPController(GeoLocationService geoLocationService, LogService logService)
        {
            _geoLocationService = geoLocationService;
            _logService = logService;
        }

        // Endpoint to check if an IP is blocked
        [HttpGet("check/{ip}")]
        public async Task<IActionResult> CheckIP(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return BadRequest("IP address cannot be null or empty.");

            try
            {
                var geoLocationResponse = await _geoLocationService.GetGeoLocationAsync(ip);

                // Add log entry for this IP attempt
                var logEntry = new LogEntry
                {
                    IpAddress = ip,
                    AttemptTime = DateTime.UtcNow,
                    CountryCode = geoLocationResponse.CountryCode // Assuming the API returns the country code
                };
                _logService.AddLogEntry(logEntry);

                return Ok(geoLocationResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching geolocation details: {ex.Message}");
            }
        }

        // Endpoint to log an IP attempt manually (if needed)
        [HttpPost("log")]
        public IActionResult LogIPAttempt([FromBody] LogEntry request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.IpAddress))
                return BadRequest("IP address is required.");

            try
            {
                request.AttemptTime = DateTime.UtcNow; // Ensure the log time is set
                _logService.AddLogEntry(request);
                return Ok($"Attempt logged for IP {request.IpAddress}.");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Endpoint to get logs with pagination
        [HttpGet("logs")]
        public IActionResult GetLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var logs = _logService.GetLogEntries(page, pageSize);
                return Ok(logs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
