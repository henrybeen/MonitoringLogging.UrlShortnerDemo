using Demo.MonitoringLogging.Functions.Main;
using Demo.MonitoringLogging.Functions.Main.Configuration;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Demo.MonitoringLogging.Functions.Shared.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Demo.MonitoringLogging.Functions.Main
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services
                .AddSingleton<ITelemetryInitializer, TelemetryInitializer>()
                .AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>()
                .AddScoped<IAddAliasService, AddAliasService>()
                .AddScoped<IGetAliasService, GetAliasService>()
                .AddScoped<IGetAliasStatisticsService, GetAliasStatisticsService>();
        }
    }
}
