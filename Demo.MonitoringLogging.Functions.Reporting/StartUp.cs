using Demo.MonitoringLogging.Functions.Reporting;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Demo.MonitoringLogging.Functions.Shared.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Demo.MonitoringLogging.Functions.Reporting
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services
                .AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>()
                .AddScoped<ISqlConnectionFactory, SqlConnectionFactory>()
                .AddScoped<IGetAliasStatisticsService, GetAliasStatisticsService>();
        }
    }
}
