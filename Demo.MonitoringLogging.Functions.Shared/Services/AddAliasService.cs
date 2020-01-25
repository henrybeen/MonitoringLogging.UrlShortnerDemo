using System;
using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Microsoft.ApplicationInsights;

namespace Demo.MonitoringLogging.Functions.Shared.Services
{
    public class AddAliasService : IAddAliasService
    {
        private static readonly Random Random = new Random();
        private readonly IRedisConnectionFactory _redisConnectionFactory;
        private readonly TelemetryClient _telemetryClient;

        public AddAliasService(IRedisConnectionFactory redisConnectionFactory, TelemetryClient telemetryClient)
        {
            _redisConnectionFactory = redisConnectionFactory;
            _telemetryClient = telemetryClient;
        }

        public async Task AddAliasAsync(AddAliasProperties addAliasProperties, DateTime aliasAddedDateTime)
        {
            var intentionalDelay = Random.Next(3000);
            await Task.Delay(intentionalDelay);

            var redisConnection = _redisConnectionFactory.GetConnection();

            if (await redisConnection.KeyExistsAsync(addAliasProperties.Alias))
            {
                _telemetryClient
                    .GetMetric("AliasOverwrittenCount")
                    .TrackValue(1);
            }

            await redisConnection.HashSetAsync(addAliasProperties.Alias, "url", addAliasProperties.Url);

            var aliasAddedToRedisDelay = DateTime.UtcNow - aliasAddedDateTime;

            _telemetryClient
                .GetMetric("AliasAddedToRedisCacheDelay")
                .TrackValue(aliasAddedToRedisDelay.TotalMilliseconds);
        }
    }
}