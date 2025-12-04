using System.Data;
using Microsoft.Data.SqlClient;

namespace ETL.Task.Database;

using System.Threading.Tasks;

public sealed class DbConnectionFactory
{
    private readonly string _connectionString;
    
    public DbConnectionFactory(string connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);
        _connectionString = connectionString;
    }
    
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}