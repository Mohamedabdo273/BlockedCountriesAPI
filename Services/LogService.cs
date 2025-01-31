using System.Collections.Concurrent;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services.IServices;

namespace BlockedCountriesAPI.Services
{
    public class LogService : ILogService
    {
        private readonly ConcurrentQueue<LogEntry> _logs = new();
        private const int MaxLogEntries = 1000; 
        public void AddLogEntry(LogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry), "Log entry cannot be null.");

            _logs.Enqueue(entry);

            // إزالة السجلات القديمة إذا تجاوزت الحد الأقصى
            while (_logs.Count > MaxLogEntries)
                _logs.TryDequeue(out _);
        }

        public IEnumerable<LogEntry> GetLogEntries(int page)
        {
             int pageSize = 10;
            if (page <= 0)
                throw new ArgumentException("Page and page size must be greater than zero.");

            return _logs.Reverse()
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int GetTotalLogCount() => _logs.Count;
    }
}
