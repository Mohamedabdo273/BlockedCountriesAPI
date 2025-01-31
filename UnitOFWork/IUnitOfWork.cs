using BlockedCountriesAPI.Repositories.IRepository;

namespace BlockedCountriesAPI.UnitOFWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBlockedCountryRepository blockedCountryRepository { get; }
        ILogRepository LogRepository    { get; }
    }
}
