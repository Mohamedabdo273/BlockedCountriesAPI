using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BlockedCountriesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IPController : ControllerBase
    {
        private readonly IGeoLocationService _geoLocationService;
        private readonly ILogService _logService;
        private readonly ICountryBlockService _countryBlockService;

        public IPController(IGeoLocationService geoLocationService, ILogService logService, ICountryBlockService countryBlockService)
        {
            _geoLocationService = geoLocationService;
            _logService = logService;
            _countryBlockService = countryBlockService;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> LookupIP([FromQuery] string ipAddress = null)
        {
            
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (ipAddress == "::1")
                {
                    ipAddress = "127.0.0.1";
                }
            }          
            // Validate IP address
            if (string.IsNullOrWhiteSpace(ipAddress) || !IPAddress.TryParse(ipAddress, out _))
            {
                return BadRequest("Invalid IP address.");
            }
            try
            {
                var geoLocationResponse = await _geoLocationService.GetGeoLocationAsync(ipAddress);
                return Ok(geoLocationResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching geolocation details: {ex.Message}");
            }
        }
        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlockedIP()
        {
            string ipAddress = GetCallerIpAddress();

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return BadRequest("Unable to fetch IP address.");
            }

            try
            {
                var geoLocationResponse = await _geoLocationService.GetGeoLocationAsync(ipAddress);
                bool isBlocked = _countryBlockService.IsCountryBlocked(geoLocationResponse.CountryCode);

                var logEntry = new LogEntry
                {
                    IpAddress = ipAddress,
                    AttemptTime = DateTime.UtcNow,
                    CountryCode = geoLocationResponse.CountryCode,
                    BlockedStatus = isBlocked,
                    UserAgent = Request.Headers["User-Agent"]
                };

                _logService.AddLogEntry(logEntry);

                return Ok(new { IsBlocked = isBlocked, GeoLocation = geoLocationResponse });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("logs/blocked-attempts")]
        public IActionResult GetBlockedAttempts([FromQuery] int page = 1)
        {
            try
            {
                var blockedLogs = _logService.GetLogEntries(page).Where(log => log.BlockedStatus).ToList();
                return Ok(blockedLogs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetCallerIpAddress()
        {
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedIp = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                ipAddress = forwardedIp.Split(',')[0].Trim(); 
            }

            return ipAddress;
        }
    }
}
