using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Demo.MonitoringLogging.Functions.Shared.Persistence
{
    public interface ISqlConnectionFactory
    {
        Task<SqlConnection> CreateConnectionAsync();
    }
}