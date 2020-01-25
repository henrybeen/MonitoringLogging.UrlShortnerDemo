using System.Threading.Tasks;

namespace Demo.MonitoringLogging.Functions.Shared.Services
{
    public interface IGetAliasService
    {
        Task<AddAliasProperties> GetAliasAsync(string alias);
    }
}
