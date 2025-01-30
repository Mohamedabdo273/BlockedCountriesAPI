using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.IRepository;
using BlockedCountriesAPI.Services.IServices;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockedCountriesAPI.Services
{
    public class CountryBlockService : ICountryBlockService
    {
        private readonly IBlockedCountryRepository _blockedCountryRepository;
        private readonly ILogService _logService;
        private readonly ILogger<CountryBlockService> _logger;

        public CountryBlockService(IBlockedCountryRepository blockedCountryRepository, ILogService logService, ILogger<CountryBlockService> logger)
        {
            _blockedCountryRepository = blockedCountryRepository;
            _logService = logService;
            _logger = logger;
        }

        public bool BlockCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            try
            {
                var existingBlock = _blockedCountryRepository.GetByCountryCode(countryCode);
                if (existingBlock != null)
                {
                    _logger.LogWarning($"Country {countryCode} is already blocked.");
                    return false;
                }
            }
            catch (KeyNotFoundException)
            {
                // Country is not found, so we can add it
            }

            _blockedCountryRepository.Add(new BlockedCountry
            {
                CountryCode = countryCode,
                BlockDate = DateTime.UtcNow,
                BlockExpiryDate = null
            });

            _logger.LogInformation($"Country {countryCode} has been blocked.");
            return true;
        }

        public bool UnblockCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            var blockedCountry = _blockedCountryRepository.GetByCountryCode(countryCode);
            if (blockedCountry == null)
            {
                _logger.LogWarning($"Country {countryCode} is not blocked.");
                return false;
            }
            
            // Remove the blocked country
            _blockedCountryRepository.Delete(blockedCountry);
            _logger.LogInformation($"Country {countryCode} has been unblocked.");
            return true;
        }
        public IEnumerable<BlockedCountry> GetBlockedCountries(int page, string searchTerm = null)
        {
            int pageSize = 10;
            if (page <= 0 )
                throw new ArgumentException("Page must be greater than zero.");

            var query = _blockedCountryRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                
                query = query.Where(c =>
                    c.CountryCode.Contains(searchTerm) ||
                    (c.CountryName?.Contains(searchTerm) == true)

                );
            }

            return query
                .OrderBy(c => c.CountryCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        public bool IsCountryBlocked(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            try
            {
                var blockedCountry = _blockedCountryRepository.GetByCountryCode(countryCode);
                if (blockedCountry == null)
                    return false;

                if (blockedCountry.BlockExpiryDate.HasValue && blockedCountry.BlockExpiryDate < DateTime.UtcNow)
                {
                    // If the block has expired, remove it and return false
                    _blockedCountryRepository.Delete(blockedCountry);
                    _logger.LogInformation($"Country {countryCode} block has expired and was removed.");
                    return false;
                }

                return true;
            }
            catch (KeyNotFoundException)
            {
                // If the country isn't found, it is not blocked
                return false;
            }
        }

        public void TemporarilyBlockCountry(TemporarilyBlockCountryRequest temporarilyBlockCountryRequest)
        {
            if (string.IsNullOrWhiteSpace(temporarilyBlockCountryRequest.CountryCode))
                throw new ArgumentException("Country code cannot be null or empty.");
            if (temporarilyBlockCountryRequest.DurationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than zero.");

            // Check if the country is already temporarily blocked
            var existingBlockedCountry = _blockedCountryRepository.GetByCountryCode(temporarilyBlockCountryRequest.CountryCode);
            if (existingBlockedCountry != null && existingBlockedCountry.BlockExpiryDate > DateTime.UtcNow)
            {
                throw new InvalidOperationException($"Country {temporarilyBlockCountryRequest.CountryCode} is already temporarily blocked.");
            }

            _blockedCountryRepository.Add(new BlockedCountry
            {
                CountryCode = temporarilyBlockCountryRequest.CountryCode,
                BlockDate = DateTime.UtcNow,
                BlockExpiryDate = DateTime.UtcNow.AddMinutes(temporarilyBlockCountryRequest.DurationMinutes)
            });

            _logger.LogInformation($"Country {temporarilyBlockCountryRequest.CountryCode} is temporarily blocked for {temporarilyBlockCountryRequest.DurationMinutes} minutes.");
        }



    }
}
