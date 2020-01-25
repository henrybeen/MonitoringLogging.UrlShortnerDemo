using System;
using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Microsoft.ApplicationInsights;

namespace Demo.MonitoringLogging.Functions.Shared.Services
{
    public class GetAliasStatisticsService : IGetAliasStatisticsService
    {
        private readonly IRedisConnectionFactory _redisConnectionFactory;
        private readonly TelemetryClient _telemetryClient;

        public GetAliasStatisticsService(IRedisConnectionFactory redisConnectionFactory, TelemetryClient telemetryClient)
        {
            _redisConnectionFactory = redisConnectionFactory;
            _telemetryClient = telemetryClient;
        }

        public async Task<AliasStatistics> GetStatisticsAsync(string alias)
        {
            var redisConnection = _redisConnectionFactory.GetConnection();

            var aliasExists = await redisConnection.KeyExistsAsync(alias);

            if (!aliasExists)
            {
                return null;
            }

            var lastUsedTask = redisConnection.HashGetAsync(alias, "lastRequest");
            var urlTask = redisConnection.HashGetAsync(alias, "url");
            var usedCountTask = redisConnection.HashGetAsync(alias, "requestCount");

            await Task.WhenAll(lastUsedTask, urlTask, usedCountTask);

            _telemetryClient
                .GetMetric("StatisticsRequested", "Alias")
                .TrackValue("1", alias);

            return new AliasStatistics
            {
                Alias = alias,
                LastUsed = (string) lastUsedTask.Result == null ? DateTime.UtcNow : DateTime.Parse(lastUsedTask.Result),
                Url = urlTask.Result,
                UsedCount = (string)usedCountTask.Result == null ? 0 : int.Parse(usedCountTask.Result)
            };
        }
    }
}