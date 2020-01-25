using System;

namespace Demo.MonitoringLogging.Functions.Shared.Services
{
    public class AliasStatistics
    {
        public string Alias { get; set; }
        public string Url { get; set; }
        public int UsedCount { get; set; }
        public DateTime LastUsed { get; set; }
    }
}