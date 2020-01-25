using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Demo.MonitoringLogging.Functions.Main.Configuration
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        public const string CustomPropertyName = "Author";

        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry != null && telemetry is RequestTelemetry && !telemetry.Context.GlobalProperties.ContainsKey(CustomPropertyName))
            {
                telemetry.Context.GlobalProperties.Add(CustomPropertyName, "Henry Been");
            }
        }
    }
}
