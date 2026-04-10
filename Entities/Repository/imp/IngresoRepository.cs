using BlumeAPI.Entities;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class IngresoRepository : IIngresoRepository
{
    private readonly AppDbContext _context;

    public IngresoRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<Ingreso> QueryBase()
    {
        return _context.Ingresos
            .AsNoTracking()
            .Include(i => i.Taller)
            
            .Include(i => i.Articulos)
                .ThenInclude(ai => ai.Articulo)
                    .ThenInclude(a => a.Color)
            
            .Include(i => i.Articulos)
                .ThenInclude(ai => ai.Articulo)
                    .ThenInclude(a => a.SubFamilia)
            
            .Include(i => i.Articulos)
                .ThenInclude(ai => ai.Articulo)
                    .ThenInclude(a => a.Medida)
            
            .Include(i => i.Articulos)
                .ThenInclude(ai => ai.Articulo)
                    .ThenInclude(a => a.ArticuloPrecio);
    }

    public async Task<Ingreso> Crear(Ingreso ingreso)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Limpiamos el Taller para que no intente duplicarlo
            if (ingreso.Taller != null) {
                ingreso.IdTaller = ingreso.Taller.Id;
                ingreso.Taller = null;
            }

            foreach (var item in ingreso.Articulos)
            {
                var articuloMaestro = await _context.Articulos
                    .FirstOrDefaultAsync(a => a.Id == item.IdArticulo);

                if (articuloMaestro == null)
                    throw new BusinessException($"No se encontró el artículo con ID {item.IdArticulo}");

                articuloMaestro.Stock += item.Cantidad;

                item.Articulo = null;
                item.IdArticulo = articuloMaestro.Id; 
                item.Fecha = ingreso.Fecha;
                }
                
            _context.Ingresos.Add(ingreso);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return ingreso;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PagedResult<Ingreso>> GetByTaller(int idTaller, DateTime desde, DateTime hasta, EstadoIngreso? estado, int page, int pageSize)
    {
    var query = QueryBase();
        var fechaHastaFinal = hasta.Date.AddDays(1).AddTicks(-1);

        query = query.Where(i => i.IdTaller == idTaller)
                    .Where(i => i.Fecha >= desde && i.Fecha <= fechaHastaFinal)
                    .Where(i => i.Estado != EstadoIngreso.Eliminado);

        // Si 'estado' tiene un valor (no es null), filtramos por él
        if (estado.HasValue)
        {
            query = query.Where(i => i.Estado == estado.Value);
        }
        else 
        {
            // Si no mandan nada, puedes decidir si traer todos 
            // o mantener el filtro por defecto de 'Creado'
            query = query.Where(i => i.Estado == EstadoIngreso.Creado);
        }

        // 3. Contamos el total de registros ANTES de paginar
        var totalRegistros = await query.CountAsync();

        // 4. Aplicamos orden y paginación
        int skip = (page - 1) * pageSize;

        var datos = await query
            .OrderByDescending(i => i.Fecha)
            .ThenByDescending(i => i.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        // 5. Devolvemos el objeto PagedResult
        return new PagedResult<Ingreso>
        {
            Items = datos,
            TotalRecords = totalRegistros,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Ingreso> GetById(int id)
    {
        var ingreso = await QueryBase()
            .FirstOrDefaultAsync(i => i.Id == id);
        return ingreso!;
    }

    public async Task<List<Ingreso>> GetByIds(List<int> ids)
    {
        return await QueryBase()
            .Where(i => ids.Contains(i.Id))
            .ToListAsync();
    }

    public async Task<bool> Existe(int id)
    {
        return await _context.Ingresos.AnyAsync(i => i.Id == id);
    }

    public async Task CrearDetallesIngresoPedidoProduccion(List<PedidoProduccionIngresoDetalle> detalles)
    {
        foreach (var detalle in detalles)
        {
            // Limpieza de navegación para evitar re-insertar objetos maestros
            detalle.PedidoProduccion = null;
            detalle.Ingreso = null;
            detalle.Articulo = null;
            detalle.Presupuesto = null;
        }

        _context.AddRange(detalles);
        await _context.SaveChangesAsync();
    }

    public async Task<List<PedidoProduccionIngresoDetalle>> GetDetallesPPI(int idIngreso)
    {
        return await _context.Set<PedidoProduccionIngresoDetalle>()
            .AsNoTracking()

            .Include(d => d.Articulo)
                .ThenInclude(a => a.Color)
            .Include(d => d.Articulo)
                .ThenInclude(a => a.Medida)
            .Include(d => d.Articulo)
                .ThenInclude(a => a.SubFamilia)

            .Include(d => d.PedidoProduccion)

            .Include(d => d.Presupuesto)
                .ThenInclude(presu => presu.Cliente)

            .Include(d => d.Ingreso)
                .ThenInclude(i => i.Taller)

            .Where(d => d.IdIngreso == idIngreso)
            .ToListAsync();
    }
    public async Task Actualizar(Ingreso ingresoRecibido)
    {
        var ingresoExistente = await _context.Ingresos
            .AsTracking()
            .Include(i => i.Articulos)
            .FirstOrDefaultAsync(i => i.Id == ingresoRecibido.Id);

        if (ingresoExistente == null) throw new Exception("Ingreso no encontrado");

        // 1. Revertimos el stock viejo antes de aplicar el nuevo
        foreach (var artViejo in ingresoExistente.Articulos)
        {
            var maestro = await _context.Articulos.FindAsync(artViejo.IdArticulo);
            if (maestro != null) maestro.Stock -= artViejo.Cantidad;
        }

        // 2. Sincronizamos cabecera e IDs (Taller)
        ingresoRecibido.Taller = null; 
        _context.Entry(ingresoExistente).CurrentValues.SetValues(ingresoRecibido);

        // 3. Borramos artículos que ya no están y actualizamos/agregamos los nuevos
        _context.ArticulosIngreso.RemoveRange(ingresoExistente.Articulos);
        
        foreach (var nuevoArt in ingresoRecibido.Articulos)
        {
            nuevoArt.Id = 0; // Para que genere IDs nuevos
            nuevoArt.IdIngreso = ingresoExistente.Id;
            
            // Impactamos el Stock Maestro con la nueva cantidad
            var maestro = await _context.Articulos.FindAsync(nuevoArt.IdArticulo);
            if (maestro != null) maestro.Stock += nuevoArt.Cantidad;

            nuevoArt.Articulo = null;
            ingresoExistente.Articulos.Add(nuevoArt);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(Ingreso ingreso)
    {
        var entry = _context.Entry(ingreso);
        ingreso.Estado = EstadoIngreso.Eliminado;
        entry.Property(x => x.Estado).IsModified = true;
    }

}