using System.Data;
using BlumeAPI.Entities.clases.modelo;
using BlumeAPI.Repositories;
using Dapper;

public class ColorRepository : IColorRepository
{
private readonly IDbConnectionFactory _factory;

    public ColorRepository(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

public async Task<Color?> GetById(int idColor)
    {
        using var conn = _factory.CreateConnection();

        var sql = @"
            SELECT
                ""ID_COLOR"" AS ""Id"",
                ""CODIGO"",
                ""DESCRIPCION"",
                ""HEXA"" AS ""ColorHexa""
            FROM ""COLOR""
            WHERE ""ID_COLOR"" = @IdColor;
        ";

        return await conn.QueryFirstOrDefaultAsync<Color>(sql, new { IdColor = idColor });
    }
}
