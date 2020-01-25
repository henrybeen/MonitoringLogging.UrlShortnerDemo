using System.Linq;
using System.Threading.Tasks;
using Demo.MonitoringLogging.Functions.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Demo.MonitoringLogging.Functions.Main.Functions
{
    public class GetStatistics
    {
        private readonly ILogger<GetStatistics> _logger;
        private readonly IGetAliasStatisticsService _getStatisticsServiceService;

        public GetStatistics(ILogger<GetStatistics> logger, IGetAliasStatisticsService getStatisticsServiceService)
        {
            _logger = logger;
            _getStatisticsServiceService = getStatisticsServiceService;
        }

        [FunctionName(nameof(GetStatistics))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request)
        {
            _logger.LogDebug("Entering GetAlias.Run");

            if (!request.Query.ContainsKey("alias"))
            {
                return new BadRequestResult();
            }

            var alias = request.Query["alias"].First();
            var aliasStatistics = await _getStatisticsServiceService.GetStatisticsAsync(alias);

            if (aliasStatistics == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(aliasStatistics);
        }
    }
}
