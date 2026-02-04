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
    public async Task<List<ArticuloFacturaEntity>> GetFacturadosByArticulo(int idArticulo)
    {

        using var conn = _factory.CreateConnection();

        var sql = @"
            SELECT
                ""ID_ARTICULO_FACTURA"" AS IdArticuloFactura,
                ""ID_ARTICULO""         AS IdArticulo,
                ""ID_FACTURA""          AS IdFactura,
                ""CODIGO""              AS Codigo,
                ""DESCRIPCION""         AS Descripcion,
                ""CANTIDAD""            AS Cantidad,
                ""PRECIO_UNITARIO""     AS PrecioUnitario,
                ""DESCUENTO""           AS Descuento,
                ""FECHA_CREACION""      AS FechaCreacion
            FROM ""ARTICULO_FACTURA""
            WHERE ""ID_ARTICULO"" = @IdArticulo
            ORDER BY ""FECHA_CREACION"" DESC;
        ";

        var result = await conn.QueryAsync<ArticuloFacturaEntity>(
            sql,
            new { IdArticulo = idArticulo }
        );

        return result.ToList();
    }

    public async Task<List<ArticuloIngresoEntity>> GetIngresadosByArticulo(int idArticulo)
    {
        using var conn = _factory.CreateConnection();

        var sql = @"
            SELECT
                ""ID_ARTICULO_INGRESO"" AS IdArticuloIngreso,
                ""ID_ARTICULO""         AS IdArticulo,
                ""ID_INGRESO""          AS IdIngreso,
                ""CODIGO""              AS Codigo,
                ""DESCRIPCION""         AS Descripcion,
                ""CANTIDAD""            AS Cantidad,
                ""FECHA_INGRESO""       AS FechaIngreso,
                ""FECHA_CREACION""      AS FechaCreacion
            FROM ""ARTICULO_INGRESO""
            WHERE ""ID_ARTICULO"" = @IdArticulo
            ORDER BY ""FECHA_INGRESO"" DESC;
        ";

        var result = await conn.QueryAsync<ArticuloIngresoEntity>(
            sql,
            new { IdArticulo = idArticulo }
        );

        return result.ToList();
    }

}
