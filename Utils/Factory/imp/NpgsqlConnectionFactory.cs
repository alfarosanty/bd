using System.Data;
using Npgsql;

public class NpgsqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(IConfiguration config)
    {
    var connectionString = config.GetConnectionString(
    "BDPruebasPCEri"
    //"BDPruebas"
    //"BDProduccion"
        );
    _connectionString = connectionString ?? throw new ArgumentNullException("Connection string is null"); 
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}
