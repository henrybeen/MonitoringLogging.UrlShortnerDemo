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
    public class GetAlias
    {
        private readonly ILogger<GetAlias> _logger;
        private readonly IGetAliasService _getAliasService;

        public GetAlias(ILogger<GetAlias> logger, IGetAliasService getAliasService)
        {
            _logger = logger;
            _getAliasService = getAliasService;
        }

        [FunctionName(nameof(GetAlias))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request)
        {
            _logger.LogDebug("Entering GetAlias.Run");

            if (!request.Query.ContainsKey("alias"))
            {
                return new BadRequestResult();
            }

            var alias = request.Query["alias"].First();

            var aliasDetails = await _getAliasService.GetAliasAsync(alias);

            if (aliasDetails == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(aliasDetails.Url);
        }
    }
}
