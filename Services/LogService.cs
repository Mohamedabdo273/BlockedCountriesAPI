using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockedCountriesAPI.Services
{
    public class LogService : ILogService
    {
        private readonly List<LogEntry> _logs;
        private readonly object _lock = new object();

        public LogService()
        {
            _logs = new List<LogEntry>();
        }

        // Add a log entry
        public void AddLogEntry(LogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry), "Log entry cannot be null.");

            lock (_lock)
            {
                _logs.Add(entry);
            }
        }

        // Get log entries with pagination
        public IEnumerable<LogEntry> GetLogEntries(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
                throw new ArgumentException("Page and page size must be greater than zero.");

            lock (_lock)
            {
                return _logs
                    .OrderByDescending(log => log.AttemptTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        // Get the total number of log entries
        public int GetTotalLogCount()
        {
            lock (_lock)
            {
                return _logs.Count;
            }
        }
    }
}
