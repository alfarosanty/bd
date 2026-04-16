
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
                .ThenInclude(c=>c.CondicionFiscal)
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

    public void Crear(Presupuesto presupuesto)
    {
        if (presupuesto.Cliente != null) 
        {
            presupuesto.IdCliente = presupuesto.Cliente.Id;
            presupuesto.Cliente = null;
        }
        
        if (presupuesto.EstadoPresupuesto != null)
        {
            presupuesto.IdEstado = presupuesto.EstadoPresupuesto.Id;
            presupuesto.EstadoPresupuesto = null;
        }

        if (presupuesto.Articulos != null)
        {
            foreach (var item in presupuesto.Articulos)
            {
                item.FechaCreacion = DateTime.Now;
                
                if (item.Articulo != null)
                {
                    item.IdArticulo = item.Articulo.Id;
                    item.Articulo = null; 
                }
                
            }
        }

        _context.Presupuestos.Add(presupuesto);
    }

    public async Task Actualizar(Presupuesto presupuestoRecibido)
    {
        var exist = await _context.Presupuestos
            .Include(p => p.Articulos)
            .FirstOrDefaultAsync(p => p.Id == presupuestoRecibido.Id);

        if (exist == null) throw new NotFoundException($"No existe {presupuestoRecibido.Id}");

        exist.Total = presupuestoRecibido.Total; 
        exist.Fecha = presupuestoRecibido.Fecha;
        exist.EximirIVA = presupuestoRecibido.EximirIVA;
        exist.DescuentoGeneral = presupuestoRecibido.DescuentoGeneral;
        exist.IdCliente = presupuestoRecibido.Cliente?.Id ?? presupuestoRecibido.IdCliente;
        exist.IdEstado = presupuestoRecibido.EstadoPresupuesto?.Id ?? presupuestoRecibido.IdEstado;

        var idsRecibidos = presupuestoRecibido.Articulos?.Select(a => a.Id).ToList() ?? new List<int>();

        foreach (var artEnBD in exist.Articulos!.ToList())
        {
            if (!idsRecibidos.Contains(artEnBD.Id))
            {
                _context.ArticulosPresupuesto.Remove(artEnBD);
            }
        }

        foreach (var itemRecibido in presupuestoRecibido.Articulos!)
        {
            var artEnBD = exist.Articulos!.FirstOrDefault(a => a.Id == itemRecibido.Id && a.Id != 0);

            if (artEnBD != null)
            {
                _context.Entry(artEnBD).CurrentValues.SetValues(itemRecibido);
            }
            else
            {
                var nuevoArticulo = new ArticuloPresupuesto
                {
                    IdPresupuesto = exist.Id,
                    Cantidad = itemRecibido.Cantidad,
                    CantidadPendiente = itemRecibido.CantidadPendiente,
                    PrecioUnitario = itemRecibido.PrecioUnitario,
                    Codigo = itemRecibido.Codigo,
                    Descripcion = itemRecibido.Descripcion,
                    IdArticulo = itemRecibido.Articulo.Id,
                    Descuento = itemRecibido.Descuento                    
                };
                _context.ArticulosPresupuesto.Add(nuevoArticulo);
            }
        }

        // 3. Forzar el marcado del campo Total para asegurar que el UPDATE incluya esta columna
        _context.Entry(exist).Property(p => p.Total).IsModified = true;

        await _context.SaveChangesAsync();
    }
}