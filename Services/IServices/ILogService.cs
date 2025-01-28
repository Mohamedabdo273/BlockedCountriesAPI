using BlockedCountriesAPI.Models;

namespace BlockedCountriesAPI.Services.IServices
{
    public interface ILogService
    {
        void AddLogEntry(LogEntry entry); // Add a log entry.
        IEnumerable<LogEntry> GetLogEntries(int page, int pageSize); // Get paginated log entries.
        int GetTotalLogCount(); // Get the total number of log entries.
    }

}
