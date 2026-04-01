using Microsoft.EntityFrameworkCore;
using BlumeAPI.Repository;
using BlumeAPI.Repository;
using BlumeAPI.Entities;
public class FacturaRepository : IFacturaRepository
{
private readonly IDbConnectionFactory _factory;
private readonly AppDbContext _context;

public FacturaRepository(AppDbContext context, IDbConnectionFactory factory)
{
    _context = context;
    _factory = factory;
}    

// ORM METHODS

private IQueryable<Factura> QueryBase()
    {
        return _context.Facturas
            .AsNoTracking()
            .Include(f => f.Cliente)
            .Include(f => f.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.Color)
            .Include(f => f.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.Medida)
            .Include(f => f.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.ArticuloPrecio)
            .Include(f => f.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.SubFamilia);
    }

public async Task<PagedResult<Factura>> GetAll(
    DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize)
{
    var query = QueryBase()
        .Where(f => f.FechaFactura >= desde && f.FechaFactura <= hasta);

    // Filtro inteligente para ARCA / Locales
    if (facturadoARCA.HasValue)
    {
        query = query.Where(f => facturadoARCA.Value 
            ? f.PuntoDeVenta == 5 
            : f.PuntoDeVenta != 5);
    }

    var totalRecords = await query.CountAsync();

    var items = await query
        .OrderByDescending(f => f.FechaFactura)
        .Skip((page - 1) * pageSize) 
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Factura>
    {
        Items = items,
        TotalRecords = totalRecords,
        Page = page,
        PageSize = pageSize
    };
}

public async Task<PagedResult<Factura>> GetByCliente(
    int idCliente, DateTime desde, DateTime hasta, bool? facturadoARCA, int page, int pageSize)
{
    // 1. Base de la consulta con filtros obligatorios
    var query = QueryBase()
        .Where(f => f.IdCliente == idCliente && f.FechaFactura >= desde && f.FechaFactura <= hasta);

    // 2. Filtro opcional de ARCA (Soberbio: maneja True, False y Todos/Null)
    if (facturadoARCA.HasValue)
    {
        query = query.Where(f => facturadoARCA.Value 
            ? f.PuntoDeVenta == 5 
            : f.PuntoDeVenta != 5);
    }

    // 3. Contamos el total antes de paginar
    var totalRecords = await query.CountAsync();

    // 4. Pagina y ordena (usando 'f' para consistencia)
    var items = await query
        .OrderByDescending(f => f.FechaFactura) 
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Factura>
    {
        Items = items,
        TotalRecords = totalRecords,
        Page = page,
        PageSize = pageSize
    };
}
public async Task<Factura?> GetByIdAsync(int idFactura)
{
    // Usamos el nombre exacto de la propiedad en la Entity: ArticulosFactura
    var query = _context.Facturas
        .AsNoTracking()
        .Include(f => f.Cliente)
        .Include(f => f.Articulos) 
            .ThenInclude(af => af.Articulo)
                .ThenInclude(a => a.Color)
        .Include(f => f.Articulos)
            .ThenInclude(af => af.Articulo)
                .ThenInclude(a => a.Medida)
        .Include(f => f.Articulos)
            .ThenInclude(af => af.Articulo)
                .ThenInclude(a => a.ArticuloPrecio)
        .Include(f => f.Articulos)
            .ThenInclude(af => af.Articulo)
                .ThenInclude(a => a.SubFamilia)
        .Where(f => f.Id == idFactura);

    // Para ver el SQL en la consola de debug
    var sql = query.ToQueryString(); 

    return await query.FirstOrDefaultAsync();
}

    // ============================
    // NOTA DE CRÉDITO
    // ============================
public async Task<int> InsertarNotaCreditoAsync(NotaDeCredito notaDeCredito)
{
    try
    {
            
    // Guardamos los artículos y los desvinculamos temporalmente
    var articulos = notaDeCredito.Articulos;
    notaDeCredito.Articulos = null;

    if (notaDeCredito.Cliente != null)
        _context.Entry(notaDeCredito.Cliente).State = EntityState.Unchanged;

    _context.NotasDeCredito.Add(notaDeCredito);
    await _context.SaveChangesAsync();

    // Restauramos los artículos
    notaDeCredito.Articulos = articulos;

    return notaDeCredito.Id;
    }
    catch (Exception ex)
    {
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"❌ Inner: {ex.InnerException?.Message}");
    throw;
    }
}

public async Task InsertarArticulosNotaCreditoAsync(List<ArticuloNotaCredito> articulos)
{
    foreach (var art in articulos)
    {
        if (art.Articulo != null)
            _context.Entry(art.Articulo).State = EntityState.Unchanged;
    }

    await _context.ArticulosNotaCredito.AddRangeAsync(articulos);
    await _context.SaveChangesAsync();
}
public async Task ActualizarNotaDeCreditoAfipAsync(int id, AfipResponse respuesta)
{
    Console.WriteLine($"🔍 Buscando nota con id: {id}");
    
    var notaEnTracker = _context.ChangeTracker.Entries<NotaDeCredito>()
        .FirstOrDefault(e => e.Entity.Id == id);
    Console.WriteLine($"🔍 En tracker: {(notaEnTracker == null ? "NO" : "SÍ")}");

    var nota = notaEnTracker?.Entity ?? await _context.NotasDeCredito.FindAsync(id);
    Console.WriteLine($"🔍 Nota encontrada: {(nota == null ? "NO" : "SÍ, Id=" + nota.Id)}");

    if (nota == null)
        throw new Exception($"No se encontró la nota de crédito con id {id}.");

    nota.Cae = long.Parse(respuesta.Cae!);
    nota.FechaVencimientoCae = respuesta.CaeVencimiento;
    nota.NumeroComprobante = int.Parse(respuesta.numeroComprobante);

    await _context.SaveChangesAsync();
}

}
