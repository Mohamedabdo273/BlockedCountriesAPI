using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using BlockedCountriesAPI.Models;
using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.Repositories
{
    public class LogRepository : RepositoryBase<LogEntry>, ILogRepository
    {
        private readonly List<LogEntry> _logEntries;

        public LogRepository()
        {
            _logEntries = new List<LogEntry>();
        }

        protected override object GetId(LogEntry entity)
        {
            return entity.Id; // استخدام Id كـ identifier
        }

        // إضافة سجل جديد
        public override void Add(LogEntry entity)
        {
            _logEntries.Add(entity);
        }

        // استرجاع سجلات المحاولات الفاشلة
        public override IEnumerable<LogEntry> GetAll(Expression<Func<LogEntry, bool>>? expression = null)
        {
            var query = _logEntries.AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }
            return query.ToList();
        }
    }
}
