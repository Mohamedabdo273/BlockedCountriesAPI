using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.IRepository;
using BlockedCountriesAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockedCountriesAPI.Services
{
    public class CountryBlockService : ICountryBlockService
    {
        private readonly IBlockedCountryRepository _blockedCountryRepository;

        // Log Service للاعتماديات الخارجية
        private readonly ILogService _logService;

        public CountryBlockService(IBlockedCountryRepository blockedCountryRepository, ILogService logService)
        {
            _blockedCountryRepository = blockedCountryRepository;
            _logService = logService;
        }

        public bool BlockCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            var existingCountry = _blockedCountryRepository.GetByCountryCode(countryCode);
            if (existingCountry != null)
            {
                _logService.AddLogEntry(new LogEntry
                {
                    CountryCode = countryCode,
                    AttemptTime = DateTime.UtcNow,
                });

                throw new InvalidOperationException("Country is already blocked.");
            }

            _blockedCountryRepository.Add(new BlockedCountry
            {
                CountryCode = countryCode,
                BlockDate = DateTime.UtcNow,
                BlockExpiryDate = null // حظر دائم
            });
            return true;
        }

        public bool UnblockCountry(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            var countryToUnblock = _blockedCountryRepository.GetByCountryCode(countryCode);
            if (countryToUnblock == null)
                throw new InvalidOperationException("Country is not blocked.");

            _blockedCountryRepository.Delete(countryToUnblock);
            return true;
        }

        public IEnumerable<BlockedCountry> GetBlockedCountries(int page, int pageSize, string searchTerm = null)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Page and page size must be greater than zero.");

            var query = _blockedCountryRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.CountryCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.CountryName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true)
                );
            }

            return query
                .OrderBy(c => c.CountryCode) // إضافة ترتيب لتحسين عرض النتائج
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public bool IsCountryBlocked(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");

            var blockedCountry = _blockedCountryRepository.GetByCountryCode(countryCode);

            if (blockedCountry != null)
            {
                // إذا كان الحظر مؤقتًا وتم انتهاؤه، نقوم بتحديث الحالة أو حذف السجل
                if (blockedCountry.IsExpired)
                {
                    _blockedCountryRepository.Delete(blockedCountry); // أو يمكنك تعديل الحقل بدلًا من الحذف
                    return false;
                }
            }

            return blockedCountry != null;
        }


        public void TemporarilyBlockCountry(string countryCode, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.");
            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than zero.");

            var existingCountry = _blockedCountryRepository.GetByCountryCode(countryCode);
            if (existingCountry != null)
                throw new InvalidOperationException("Country is already blocked.");

            _blockedCountryRepository.Add(new BlockedCountry
            {
                CountryCode = countryCode,
                BlockDate = DateTime.UtcNow,
                BlockExpiryDate = DateTime.UtcNow.AddMinutes(durationMinutes)
            });
        }
    }
}
