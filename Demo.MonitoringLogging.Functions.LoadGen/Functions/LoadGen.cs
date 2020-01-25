using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demo.MonitoringLogging.Functions.LoadGen.Functions
{
    public class LoadGen
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly Random Random = new Random();

        private readonly ILogger<LoadGen> _logger;
        private readonly IConfiguration _configuration;

        public LoadGen(ILogger<LoadGen> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [FunctionName(nameof(LoadGen))]
        public async Task Run(
            [TimerTrigger("*/2 * * * * *")]TimerInfo myTimer)
        {
            _logger.LogInformation($"StarC# Timer trigger function started at: {DateTime.Now}");

            var alias = CreateAlias();

            await PostAsync($"{_configuration["MainFunctions.BaseUrl"]}/api/AddAlias", new
            {
                Alias = alias,
                Url = "https://henrybeen.nl"
            });

            await Task.Delay(2200);

            await Task.WhenAll(
                GetAsync($"{_configuration["MainFunctions.BaseUrl"]}/api/GetAlias?alias={alias}"),
                GetAsync($"{_configuration["MainFunctions.BaseUrl"]}/api/GetAlias?alias={alias}"),
                GetAsync($"{_configuration["MainFunctions.BaseUrl"]}/api/GetAlias?alias={alias}"),
                GetAsync($"{_configuration["MainFunctions.BaseUrl"]}/api/GetAlias?alias={alias}"),
                GetAsync($"{_configuration["MainFunctions.BaseUrl"]}/api/GetAlias?alias={alias}"));

            await GetAsync($"{_configuration["MainFunctions.BaseUrl"]}/api/GetStatistics?alias={alias}");

            _logger.LogInformation($"StarC# Timer trigger function completed at: {DateTime.Now}");

        }

        private string CreateAlias()
        {
            return $"alias-{Random.Next(0, 1000)}";
        }


        private async Task GetAsync(string url)
        {
            await HttpClient.GetAsync(url);
        }

        private async Task PostAsync(string url, object content)
        {
            var json = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8);
            await HttpClient.PostAsync(url, json);
        }
    }
}
