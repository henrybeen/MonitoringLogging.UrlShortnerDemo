using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Demo.MonitoringLogging.Functions.Shared.Persistence
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        public const string CreateTableQuery = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='aliases' and xtype='U')
        CREATE TABLE aliases (
            id INT NOT NULL IDENTITY(1,1),
            alias NVARCHAR(64) NOT NULL,
            statisticsAt DATETIME2 NOT NULL,
            url NVARCHAR(1024) NOT NULL,
            usedCount INT NOT NULL,
            lastUsed DATETIME2 NOT NULL
        )";

        private readonly Semaphore _semaphore = new Semaphore(1, 1);
        private readonly IConfiguration _configuration;

        private bool _databasesIsInitialized;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SqlConnection> CreateConnectionAsync()
        {
            await InitDatabaseIfNeededAsync();

            return CreateConnection();
        }

        private async Task InitDatabaseIfNeededAsync()
        {
            if (_databasesIsInitialized)
            {
                return;
            }

            using (var connection = CreateConnection())
            {
                _semaphore.WaitOne();
                if (_databasesIsInitialized)
                {
                    return;
                }

                using (var command = new SqlCommand(CreateTableQuery, connection))
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    connection.Close();
                }

                _databasesIsInitialized = true;
                _semaphore.Release(1);
            }
        }

        private SqlConnection CreateConnection()
        {
            var connectionString = _configuration["SqlDatabase.ConnectionString"];
            return new SqlConnection(connectionString);
        }
    }
}