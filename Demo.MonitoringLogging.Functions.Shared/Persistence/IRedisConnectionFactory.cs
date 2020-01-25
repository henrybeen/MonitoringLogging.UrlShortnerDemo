using StackExchange.Redis;

namespace Demo.MonitoringLogging.Functions.Shared.Persistence
{
    public interface IRedisConnectionFactory
    {
        IDatabase GetConnection();
    }
}
