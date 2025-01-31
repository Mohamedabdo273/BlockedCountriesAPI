using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.UnitOFWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlockedCountryRepository _blockedCountryRepository;
        private readonly LogRepository _logRepository;

        public UnitOfWork()
        {
            // Initialize the repositories
            _blockedCountryRepository = new BlockedCountryRepository();
            _logRepository = new LogRepository();
        }

        // Implement the properties from IUnitOfWork
        public ILogRepository LogRepository => _logRepository;
        public IBlockedCountryRepository blockedCountryRepository => _blockedCountryRepository;


        // Implement IDisposable
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose any resources if needed
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}