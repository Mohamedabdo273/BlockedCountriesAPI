using System.Collections.Concurrent;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.Repositories
{
    public class LogRepository : RepositoryBase<LogEntry>, ILogRepository
    {
        protected override object GetId(LogEntry entity) => entity.IpAddress;
    }
}
