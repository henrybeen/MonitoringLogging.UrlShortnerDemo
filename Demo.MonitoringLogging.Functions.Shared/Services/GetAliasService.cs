using System;
using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Microsoft.ApplicationInsights;

namespace Demo.MonitoringLogging.Functions.Shared.Services
{
    public class GetAliasService : IGetAliasService
    {
        private readonly IRedisConnectionFactory _redisConnectionFactory;
        private readonly TelemetryClient _telemetryClient;

        public GetAliasService(IRedisConnectionFactory redisConnectionFactory, TelemetryClient telemetryClient)
        {
            _redisConnectionFactory = redisConnectionFactory;
            _telemetryClient = telemetryClient;
        }

        public async Task<AddAliasProperties> GetAliasAsync(string alias)
        {
            var redisConnection = _redisConnectionFactory.GetConnection();

            var url = (string) await redisConnection.HashGetAsync(alias, "url");

            if (url == null)
            {
                _telemetryClient
                    .GetMetric("AliasMisingCount")
                    .TrackValue(1);

                return null;
            }
            else
            {
                _telemetryClient
                    .GetMetric("AliasRequestedcount", "alias")
                    .TrackValue(1, alias);
            }

            await Task.WhenAll(
                redisConnection.HashIncrementAsync(alias, "requestCount"),
                redisConnection.HashSetAsync(alias, "lastRequest", DateTime.UtcNow.ToString("O")));

            return new AddAliasProperties
            {
                Alias = alias,
                Url = url
            };
        }
    }
}