using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Demo.MonitoringLogging.Functions.LoadGen.Functions
{
    public class Reset
    {
        private readonly ILogger<Reset> _logger;
        private readonly IRedisConnectionFactory _redisConnectionFactory;

        public Reset(ILogger<Reset> logger, IRedisConnectionFactory redisConnectionFactory)
        {
            _logger = logger;
            _redisConnectionFactory = redisConnectionFactory;
        }

        [FunctionName(nameof(Reset))]
        public async Task Run(
            [TimerTrigger("0 0 */4 * * *")]TimerInfo myTimer)
        {
            _logger.LogCritical("Resetting demo");

            var connection = _redisConnectionFactory.GetConnection();

            await connection.ExecuteAsync("FLUSHDB");

            _logger.LogCritical("Reset done");
        }
    }
}
