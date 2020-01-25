using System.Text;
using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.MonitoringLogging.Functions.Main.Functions
{
    public class AddAliasProcessor
    {
        private readonly ILogger<AddAliasProcessor> _logger;
        private readonly IAddAliasService _addAliasService;

        public AddAliasProcessor(ILogger<AddAliasProcessor> logger, IAddAliasService addAliasService)
        {
            _logger = logger;
            _addAliasService = addAliasService;
        }

        [FunctionName(nameof(AddAliasProcessor))]
        public async Task Run(
            [ServiceBusTrigger("%ServiceBus.AliasAddedTopicName%", "%ServiceBus.AliasAddedToRedisSubscriptionName%", Connection = "ServiceBus.AliasAddedListenerConnectionString")] Message message)
        {
            _logger.LogDebug("Entering AddAliasProcessor.Run");

            var json = Encoding.UTF8.GetString(message.Body);
            var alias = JsonConvert.DeserializeObject<AddAliasProperties>(json);

            await _addAliasService.AddAliasAsync(alias, message.SystemProperties.EnqueuedTimeUtc);
        }
    }
}
