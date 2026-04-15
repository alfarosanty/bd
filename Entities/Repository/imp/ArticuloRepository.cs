using System.Data;
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
    public async Task<Articulo?> GetByIdAsync(int idArticulo)
    {
        var query = _context.Articulos
            .AsNoTracking()
            .Include(a => a.Color)
            .Include(a => a.Medida)
            .Include(a => a.SubFamilia)
            .Include(a => a.ArticuloPrecio)
            .Where(a => a.Id == idArticulo);

        var sql = query.ToQueryString();

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<Articulo>> GetByIdsAsync(List<int> ids)
    {
        return await _context.Articulos
            .Where(a => ids.Contains(a.Id))
            .ToListAsync();
    }



    public async Task<ArticuloPrecio?> GetArticuloPrecioByIdAsync(int idArticuloPrecio)
    {
        var query = _context.ArticuloPrecios
            .AsNoTracking()
            .Where(p => p.Id == idArticuloPrecio);

        // 🔍 LOG DEL SQL REAL
        var sql = query.ToQueryString();

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<Articulo>> GetAllAsync(bool? habilitados)
    {
        var query = _context.Articulos
            .AsNoTracking()
            .Include(a => a.Color)
            .Include(a => a.Medida)
            .Include(a => a.SubFamilia)
            .Include(a => a.ArticuloPrecio)
            .AsQueryable();

        if (habilitados.HasValue)
        {
            query = query.Where(a => a.Habilitado == habilitados.Value);
        }

        return await query
            .OrderBy(a => a.Id)
            .ToListAsync();
    }

    public async Task<List<ArticuloPrecio>> GetAllArticulosPrecioAsync(bool? habilitados)
    {
        // 1. Empezamos con el Set y AsNoTracking (excelente para performance)
        var query = _context.Set<ArticuloPrecio>()
            .AsNoTracking()
            .AsQueryable();

        /* 2. Aplicamos el filtro de habilitados SOLAMENTE si tiene un valor
        if (habilitados.HasValue)
        {
            query = query.Where(a => a.Habilitado == habilitados.Value);
        }

        */ 
            return await query
            .OrderBy(a => a.Id)
            .ToListAsync();
    }
    /*
    public async Task<List<ArticuloEntity>> GetArticulosByArticuloPrecioIdAsync(
        int articuloPrecioId,
        bool soloHabilitados)
    {
        var query = _context.Articulos
            .AsNoTracking()
            .Where(a => a.IdArticuloPrecio == articuloPrecioId);

        if (soloHabilitados)
            query = query.Where(a => a.Habilitado);

        return await query
            .OrderBy(a => a.IdArticulo)
            .ToListAsync();
    }

    */
        // DAPPER METHODS
    public async Task<List<CartaKardexDTO>> GetFacturadosByArticulo(
        int idArticulo,
        DateTime? desde,
        DateTime? hasta)
    {
        using var conn = _factory.CreateConnection();

        var sql = @"
            SELECT
                AF.""ID_ARTICULO""      AS ""IdArticulo"",
                AF.""CODIGO""           AS ""Codigo"",
                AF.""DESCRIPCION""      AS ""Descripcion"",
                'FACTURADO'             AS ""Tipo"",
                F.""ID_FACTURA""        AS ""DocumentoId"",
                C.""RAZON_SOCIAL""      AS ""DocumentoNombre"",
                F.""FECHA_FACTURA""     AS ""Fecha"",
                AF.""CANTIDAD"" * -1    AS ""Cantidad""
            FROM ""ARTICULO_FACTURA"" AF
            JOIN ""FACTURA"" F ON AF.""ID_FACTURA"" = F.""ID_FACTURA""
            JOIN ""CLIENTE"" C ON F.""ID_CLIENTE"" = C.""ID_CLIENTE""
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

        sql += @" ORDER BY F.""FECHA_FACTURA"" DESC";

        var result = await conn.QueryAsync<CartaKardexDTO>(sql, parameters);
        return result.ToList();
    }

    public async Task<List<CartaKardexDTO>> GetIngresadosByArticulo(
        int idArticulo,
        DateTime? desde,
        DateTime? hasta)
    {
        using var conn = _factory.CreateConnection();

        var sql = @"
            SELECT
                AI.""ID_ARTICULO""      AS ""IdArticulo"",
                AI.""CODIGO""           AS ""Codigo"",
                AI.""DESCRIPCION""      AS ""Descripcion"",
                'INGRESADO'             AS ""Tipo"",
                I.""ID_INGRESO""        AS ""DocumentoId"",
                T.""RAZON_SOCIAL""      AS ""DocumentoNombre"",
                I.""FECHA_INGRESO""     AS ""Fecha"",
                AI.""CANTIDAD""         AS ""Cantidad""
            FROM ""ARTICULO_INGRESO"" AI
            JOIN ""INGRESO"" I ON AI.""ID_INGRESO"" = I.""ID_INGRESO""
            JOIN ""FABRICANTE"" T ON I.""ID_FABRICANTE"" = T.""ID_FABRICANTE""
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

        sql += @" ORDER BY I.""FECHA_INGRESO"" DESC";

        var result = await conn.QueryAsync<CartaKardexDTO>(sql, parameters);
        return result.ToList();
    }

    public async Task RestaurarStockAsync(List<IArticuloConStock> articulos, 
                                        NpgsqlConnection conn, 
                                        NpgsqlTransaction tran
                                        )
    {

        var sql = @"
            UPDATE ""ARTICULO""
            SET ""STOCK"" = ""STOCK"" + @Cantidad
            WHERE ""ID_ARTICULO"" = @IdArticulo
        ";

        var parametros = articulos
            .Where(a => a.Articulo != null)
            .Select(a => new 
            {
                Cantidad = a.Cantidad,
                IdArticulo = a.Articulo!.Id
            }).ToList();

        Console.WriteLine($"🔍 Restaurando stock de {parametros.Count} artículos:");
        foreach (var p in parametros)
            Console.WriteLine($"   IdArticulo: {p.IdArticulo}, Cantidad: {p.Cantidad}");

        await conn.ExecuteAsync(sql, parametros, tran);
    }

    public async Task Update(Articulo articulo)
    {
    
        if (articulo.ArticuloPrecio != null)
        {
            articulo.ArticuloPrecio = null;
        }

        if (articulo.Medida != null)
        {
            articulo.Medida = null;
        }

        if (articulo.Color != null)
        {
            articulo.Color = null;
        }

        if (articulo.SubFamilia != null)
        {
            articulo.SubFamilia = null;
        }

        _context.Entry(articulo).State = EntityState.Modified;

        await Task.CompletedTask;
    }


}
