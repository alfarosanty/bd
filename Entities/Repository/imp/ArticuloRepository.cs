using System.Data;
using BlumeAPI.Data.Entities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class ArticuloRepository : IArticuloRepository
{
private readonly IDbConnectionFactory _factory;
private readonly AppDbContext _context;

public ArticuloRepository(IDbConnectionFactory factory, AppDbContext context)
{
    _factory = factory;
    _context = context;
}

    // ORM METHODS
public async Task<ArticuloEntity?> GetByIdAsync(int idArticulo)
{
    var query = _context.Articulos
        .AsNoTracking()
        .Where(a => a.IdArticulo == idArticulo);

    // 🔍 LOG DEL SQL REAL
    var sql = query.ToQueryString();

    return await query.FirstOrDefaultAsync();
}


public async Task<ArticuloPrecioEntity?> GetArticuloPrecioByIdAsync(int idArticuloPrecio)
{
    var query = _context.ArticuloPrecios
        .AsNoTracking()
        .Where(p => p.IdArticuloPrecio == idArticuloPrecio);

    // 🔍 LOG DEL SQL REAL
    var sql = query.ToQueryString();

    return await query.FirstOrDefaultAsync();
}

    // DAPPER METHODS
public async Task<List<ArticuloFacturaEntity>> GetFacturadosByArticulo(
    int idArticulo,
    DateTime? desde,
    DateTime? hasta)
{
    using var conn = _factory.CreateConnection();

    var sql = @"
        SELECT
            AF.""ID_ARTICULO_FACTURA"" AS IdArticuloFactura,
            AF.""ID_ARTICULO""         AS IdArticulo,
            AF.""ID_FACTURA""          AS IdFactura,
            AF.""CODIGO""              AS Codigo,
            AF.""DESCRIPCION""         AS Descripcion,
            AF.""CANTIDAD""            AS Cantidad,
            AF.""PRECIO_UNITARIO""     AS PrecioUnitario,
            AF.""DESCUENTO""           AS Descuento,
            AF.""FECHA_CREACION""      AS FechaCreacion
        FROM ""ARTICULO_FACTURA"" AF
        JOIN ""FACTURA"" F ON AF.""ID_FACTURA"" = F.""ID_FACTURA""
        WHERE AF.""ID_ARTICULO"" = @IdArticulo
    ";

    var parameters = new DynamicParameters();
    parameters.Add("IdArticulo", idArticulo);

    if (desde.HasValue)
    {
        sql += @" AND F.""FECHA_FACTURA"" >= @Desde";
        parameters.Add("Desde", desde.Value);
    }

    if (hasta.HasValue)
    {
        sql += @" AND F.""FECHA_FACTURA"" <= @Hasta";
        parameters.Add("Hasta", hasta.Value);
    }

    sql += @" ORDER BY AF.""ID_ARTICULO_FACTURA"" DESC";

    var result = await conn.QueryAsync<ArticuloFacturaEntity>(sql, parameters);
    return result.ToList();
}

    public async Task<List<ArticuloIngresoEntity>> GetIngresadosByArticulo(
    int idArticulo,
    DateTime? desde,
    DateTime? hasta)
{
    using var conn = _factory.CreateConnection();

    var sql = @"
        SELECT
            AI.""ID_ARTICULO_INGRESO"" AS IdArticuloIngreso,
            AI.""ID_ARTICULO""         AS IdArticulo,
            AI.""ID_INGRESO""          AS IdIngreso,
            AI.""CODIGO""              AS Codigo,
            AI.""DESCRIPCION""         AS Descripcion,
            AI.""CANTIDAD""            AS Cantidad,
            AI.""FECHA_INGRESO""       AS FechaIngreso,
            AI.""FECHA_CREACION""      AS FechaCreacion
        FROM ""ARTICULO_INGRESO"" AI
        JOIN ""INGRESO"" I ON AI.""ID_INGRESO"" = I.""ID_INGRESO""
        WHERE AI.""ID_ARTICULO"" = @IdArticulo
    ";

    var parameters = new DynamicParameters();
    parameters.Add("IdArticulo", idArticulo);

    if (desde.HasValue)
    {
        sql += @" AND I.""FECHA_INGRESO"" >= @Desde";
        parameters.Add("Desde", desde.Value);
    }

    if (hasta.HasValue)
    {
        sql += @" AND I.""FECHA_INGRESO"" <= @Hasta";
        parameters.Add("Hasta", hasta.Value);
    }

    sql += @" ORDER BY AI.""ID_ARTICULO_INGRESO"" DESC";

    var result = await conn.QueryAsync<ArticuloIngresoEntity>(sql, parameters);
    return result.ToList();
}

}
