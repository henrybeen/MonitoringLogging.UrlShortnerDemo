using System;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Demo.MonitoringLogging.Functions.Shared.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.MonitoringLogging.Functions.Reporting.Functions
{
    public class AddAliasToReporting
    {
        public const string AddAliasQuery = "INSERT INTO aliases (alias, url, statisticsAt, usedCount, lastUsed) VALUES (@alias, @url, @statisticsAt, @usedCount, @lastUsed)";

        private readonly ILogger<AddAliasToReporting> _logger;
        private readonly IGetAliasStatisticsService _getAliasStatisticsService;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public AddAliasToReporting(ILogger<AddAliasToReporting> logger, IGetAliasStatisticsService getAliasStatisticsService, ISqlConnectionFactory sqlConnectionFactory)
        {
            _logger = logger;
            _getAliasStatisticsService = getAliasStatisticsService;
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        [FunctionName(nameof(AddAliasToReporting))]
        public async Task Run(
            [ServiceBusTrigger("%ServiceBus.AliasReportingQueueName%", Connection = "ServiceBus.AliasReportingConnectionString")] Message message,
            [ServiceBus("%ServiceBus.AliasReportingQueueName%", Connection = "ServiceBus.AliasReportingConnectionString")] ICollector<Message> messageBusCollector)
        {
            _logger.LogDebug("Entering AddAliasToReporting.Run");

            var requestContentJson = Encoding.UTF8.GetString(message.Body);
            var requestContent = JsonConvert.DeserializeObject<AddAliasProperties>(requestContentJson);

            var statistics = await _getAliasStatisticsService.GetStatisticsAsync(requestContent.Alias);

            if (statistics == null)
            {
                return;
            }

            using (var databaseConnection = await _sqlConnectionFactory.CreateConnectionAsync())
            {
                using (var command = new SqlCommand(AddAliasQuery, databaseConnection))
                {
                    command.Parameters.AddWithValue("url", statistics.Url);
                    command.Parameters.AddWithValue("alias", statistics.Alias);
                    command.Parameters.AddWithValue("statisticsAt", DateTime.UtcNow);
                    command.Parameters.AddWithValue("usedCount", statistics.UsedCount);
                    command.Parameters.AddWithValue("lastUsed", statistics.LastUsed);

                    await databaseConnection.OpenAsync();
                    var result = await command.ExecuteNonQueryAsync();
                    databaseConnection.Close();
                }
            }

            var newMessage = new Message(message.Body)
            {
                ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddMinutes(30)
            };

            messageBusCollector.Add(newMessage);
        }
    }
}