using System.Data;
using BlumeAPI.Entities.clases.modelo;
using BlumeAPI.Repositories;
using Dapper;

public class MedidaRepository : IMedidaRepository
{
private readonly IDbConnectionFactory _factory;

    public MedidaRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<Medida?> GetById(int idMedida)
    {
        using var conn = _factory.CreateConnection();

        var sql = @"
            SELECT
                ""ID_MEDIDA"" AS ""Id"",
                ""CODIGO"" AS Codigo,
                ""DESCRIPCION"" AS Descripcion
            FROM ""MEDIDA""
            WHERE ""ID_MEDIDA"" = @IdMedida;
        ";

        return await conn.QueryFirstOrDefaultAsync<Medida>(sql, new { IdMedida = idMedida });
    }
}
