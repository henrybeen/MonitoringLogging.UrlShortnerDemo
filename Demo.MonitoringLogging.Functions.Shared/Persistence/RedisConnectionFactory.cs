using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Demo.MonitoringLogging.Functions.Shared.Persistence
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly Lazy<ConnectionMultiplexer> _multiplexer;

        public RedisConnectionFactory(IConfiguration configuration)
        {
            _multiplexer = new Lazy<ConnectionMultiplexer>(() =>
            {
                var connectionString = configuration["Redis.ConnectionString"];
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }

        public IDatabase GetConnection()
        {
            return _multiplexer.Value.GetDatabase();
        }

        public void Dispose()
        {
            if (_multiplexer.IsValueCreated)
            {
                _multiplexer.Value.Dispose();
            }
        }
    }
}