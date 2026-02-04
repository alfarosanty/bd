using System.Data;
using BlumeAPI.Entities.clases.modelo;
using BlumeAPI.Repositories;
using Dapper;

public class SubfamiliaRepository : ISubfamiliaRepository
{
    private readonly IDbConnectionFactory _factory;

    public SubfamiliaRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

public async Task<SubFamilia?> GetById(int idSubfamilia)
{
    using var conn = _factory.CreateConnection();

    var sql = @"
        SELECT
            ""ID_SUBFAMILIA"" AS ""Id"",
            ""CODIGO"" AS Codigo,
            ""DESCRIPCION"" AS Descripcion
        FROM ""SUBFAMILIA""
        WHERE ""ID_SUBFAMILIA"" = @IdSubfamilia;
    ";

    return await conn.QueryFirstOrDefaultAsync<SubFamilia>(sql, new { IdSubfamilia = idSubfamilia });
    }
}
