using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.MonitoringLogging.Functions.Main.Functions
{
    public class AddAlias
    {
        private readonly ILogger<AddAlias> _logger;

        public AddAlias(ILogger<AddAlias> logger)
        {
            _logger = logger;
        }

        [FunctionName(nameof(AddAlias))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            [ServiceBus("%ServiceBus.AliasAddedTopicName%", Connection = "ServiceBus.AliasAddedSenderConnectionString")] ICollector<string> messageBusCollector)
        {
            _logger.LogDebug("Entering AddAlias.Run");

            var requestContentJson = await request.ReadAsStringAsync();
            var requestContent = JsonConvert.DeserializeObject<AddAliasProperties>(requestContentJson);

            if (string.IsNullOrWhiteSpace(requestContent.Alias) || string.IsNullOrWhiteSpace(requestContent.Url))
            {
                return new BadRequestResult();
            }

            messageBusCollector.Add(requestContentJson);
            return new AcceptedResult();
        }
    }
}
