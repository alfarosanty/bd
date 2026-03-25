
using BlumeAPI.Repositories;
using Microsoft.EntityFrameworkCore;

public class PresupuestoRepository : IPresupuestoRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<PresupuestoRepository> _logger;

    public PresupuestoRepository(AppDbContext context, ILogger<PresupuestoRepository> illoger)
    {
        _logger = illoger;
        _context = context;
    }

    private IQueryable<Presupuesto> QueryBase()
    {
        return _context.Presupuestos
            .AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.EstadoPresupuesto)
            .Include(p => p.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.Color)
            .Include(p => p.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.Medida)
            .Include(p => p.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.ArticuloPrecio)
            .Include(p => p.Articulos)
                .ThenInclude(a => a.Articulo)
                    .ThenInclude(a => a.SubFamilia);
    }

public async Task<PagedResult<Presupuesto>> GetAll(
    DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
{
    var query = QueryBase()
        .Where(p => p.Fecha >= desde && p.Fecha <= hasta);

    if (idEstado.HasValue && idEstado.Value > 0)
        query = query.Where(p => p.EstadoPresupuesto.Id == idEstado.Value);

    // Contamos ANTES de paginar, sobre el query ya filtrado
    var totalRecords = await query.CountAsync();

    var items = await query
        .OrderByDescending(p => p.Fecha)
        .Skip((page - 1) * pageSize)   // ← reemplaza el Take(50) hardcodeado
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Presupuesto>
    {
        Items = items,
        TotalRecords = totalRecords,
        Page = page,
        PageSize = pageSize
    };
}

public async Task<PagedResult<Presupuesto>> GetByCliente(
    int idCliente, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
{
    var query = QueryBase()
        .Where(p => p.IdCliente == idCliente && p.Fecha >= desde && p.Fecha <= hasta);

    if (idEstado.HasValue && idEstado.Value > 0)
        query = query.Where(p => p.EstadoPresupuesto.Id == idEstado.Value);

    var totalRecords = await query.CountAsync();

    var items = await query
        .OrderByDescending(p => p.Fecha)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Presupuesto>
    {
        Items = items,
        TotalRecords = totalRecords,
        Page = page,
        PageSize = pageSize
    };
}

public async Task<Presupuesto?> GetById(int id)
{
    try 
    {
        Console.WriteLine($"---> [DEBUG] Buscando presupuesto con ID: {id}");

        var presupuesto = await QueryBase()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (presupuesto == null)
        {
            Console.WriteLine($"---> [WARN] No se encontró el presupuesto con ID: {id}");
        }
        else 
        {
            Console.WriteLine($"---> [SUCCESS] Presupuesto {id} encontrado correctamente.");
        }

        return presupuesto;
    }
    catch (Exception ex) 
    {
        Console.WriteLine("#########################################");
        Console.WriteLine($"---> [ERROR FATAL] Rompió GetById({id})");
        Console.WriteLine($"---> Mensaje: {ex.Message}");
        if (ex.InnerException != null) 
        {
            Console.WriteLine($"---> Inner Exception: {ex.InnerException.Message}");
        }
        Console.WriteLine($"---> StackTrace: {ex.StackTrace}");
        Console.WriteLine("#########################################");
        throw; 
    }
}
}