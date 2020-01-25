using System.Threading.Tasks;

namespace Demo.MonitoringLogging.Functions.Shared.Services
{
    public interface IGetAliasStatisticsService
    {
        Task<AliasStatistics> GetStatisticsAsync(string alias);
    }
}