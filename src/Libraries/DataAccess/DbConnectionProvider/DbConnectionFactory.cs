using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;
using System.Data.SqlClient;

namespace DataAccess.DbConnectionProvider
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private readonly string _provider;
        public DbConnectionFactory(string connectionString, string provider)
        {
            _connectionString = connectionString;
            _provider = provider;
        }

        public async Task<DbConnection> CreateAsync(string name = null)
        {
            DbConnection dbConnection = _provider switch
            {
                "Npgsql" => new NpgsqlConnection(_connectionString),
                "SqlServer" => new SqlConnection(_connectionString),
                _ => throw new NotSupportedException($"Provider {_provider} not supported")
            };
            await dbConnection.OpenAsync();
            return dbConnection;
        }
    }
}
