using System;
using System.Threading.Tasks;

namespace Demo.MonitoringLogging.Functions.Shared.Services
{
    public interface IAddAliasService
    {
        Task AddAliasAsync(AddAliasProperties addAliasProperties, DateTime aliasAddedDateTime);
    }
}
