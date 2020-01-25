using Demo.MonitoringLogging.Functions.LoadGen;
using Demo.MonitoringLogging.Functions.Shared.Persistence;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Demo.MonitoringLogging.Functions.LoadGen
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services
                .AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
        }
    }
}
