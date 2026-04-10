
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class PresupuestoRepository : IPresupuestoRepository
{
    private readonly AppDbContext _context;

    public PresupuestoRepository(AppDbContext context)
    {
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

    public async Task<Dictionary<int, string>> GetNombresClientesPorPresupuestos(List<int> idsPresupuesto)
    {
        var idsUnicos = idsPresupuesto.Distinct().ToList();

        return await _context.Presupuestos
            .AsNoTracking()
            .Where(p => idsUnicos.Contains(p.Id))
            .Select(p => new 
            { 
                p.Id, 
                Nombre = p.Cliente != null ? p.Cliente.RazonSocial : "Stock" 
            })
            .ToDictionaryAsync(x => x.Id, x => x.Nombre);
    }

    public async Task<List<EstadoPresupuesto>> GetEstados()
    {
       return await _context.EstadosPresupuesto
            .AsNoTracking()
            .ToListAsync(); 
    }

    public async Task<int> Crear(Presupuesto presupuesto)
    {
        if (presupuesto == null)
            throw new ArgumentNullException(nameof(presupuesto));

        _context.Presupuestos.Add(presupuesto);
        await _context.SaveChangesAsync();

        return presupuesto.Id;
    }

    public async Task Actualizar(Presupuesto presupuestoRecibido)
    {
        var presupuestoExistente = await _context.Presupuestos
            .Include(p => p.Articulos)
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == presupuestoRecibido.Id);

        if (presupuestoExistente == null)
            throw new NotFoundException($"El presupuesto {presupuestoRecibido.Id} no existe.");

        // Resolver FKs si vienen como navegación
        if (presupuestoRecibido.IdCliente == 0 && presupuestoRecibido.Cliente != null)
            presupuestoRecibido.IdCliente = presupuestoRecibido.Cliente.Id;

        if (presupuestoRecibido.IdEstado == 0 && presupuestoRecibido.EstadoPresupuesto != null)
            presupuestoRecibido.IdEstado = presupuestoRecibido.EstadoPresupuesto.Id;

        // Limpiar navegaciones para que SetValues no se confunda
        presupuestoRecibido.Cliente = null;
        presupuestoRecibido.EstadoPresupuesto = null;

        _context.Entry(presupuestoExistente).CurrentValues.SetValues(presupuestoRecibido);

        // Eliminar artículos que ya no están
        foreach (var artExistente in presupuestoExistente.Articulos!.ToList())
        {
            if (!presupuestoRecibido.Articulos!.Any(a => a.Id == artExistente.Id))
                _context.Remove(artExistente);
        }

        // Actualizar o insertar artículos
        foreach (var artRecibido in presupuestoRecibido.Articulos!)
        {
            if (artRecibido.IdArticulo == 0 && artRecibido.Articulo != null)
                artRecibido.IdArticulo = artRecibido.Articulo.Id;

            var artExistente = presupuestoExistente.Articulos!
                .FirstOrDefault(a => a.Id == artRecibido.Id);

            if (artExistente != null && artRecibido.Id != 0)
            {
                artRecibido.Articulo = null;
                artRecibido.Presupuesto = null;
                _context.Entry(artExistente).CurrentValues.SetValues(artRecibido);
            }
            else
            {
                artRecibido.Id = 0;
                artRecibido.IdPresupuesto = presupuestoExistente.Id;
                artRecibido.Articulo = null;
                artRecibido.Presupuesto = null;

                presupuestoExistente.Articulos!.Add(artRecibido);
            }
        }

        await _context.SaveChangesAsync();
    }
}