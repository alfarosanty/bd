using BlumeAPI.Entities;
using BlumeAPI.Repository;
using Microsoft.EntityFrameworkCore;

public class PedidoProduccionRepository : IPedidoProduccionRepository
{
    private readonly AppDbContext _context;

    public PedidoProduccionRepository(AppDbContext context)
    {
        _context = context;
    }

    private IQueryable<PedidoProduccion> QueryBase()
    {
        return _context.PedidosProduccion
            .AsNoTracking()
            .Include(p => p.Taller)
            .Include(p => p.EstadoPedidoProduccion)
            .Include(p => p.Articulos)
                .ThenInclude(pa => pa.Articulo)
                    .ThenInclude(a => a.Color)
            .Include(p => p.Articulos)
                .ThenInclude(pa => pa.Articulo)
                    .ThenInclude(a => a.SubFamilia)
            .Include(p => p.Articulos)
                .ThenInclude(pa => pa.Articulo)
                    .ThenInclude(a => a.Medida)
            .Include(p => p.Articulos)
                .ThenInclude(pa => pa.Articulo)
                    .ThenInclude(a => a.ArticuloPrecio);
    }

    public async Task<int> Crear(PedidoProduccion pedido)
    {
        if (pedido.EstadoPedidoProduccion != null)
        {
            pedido.IdEstadoPedidoProduccion = pedido.EstadoPedidoProduccion.Id;
            pedido.EstadoPedidoProduccion = null;
        }

        if (pedido.Taller != null)
        {
            pedido.IdTaller = pedido.Taller.Id;
            pedido.Taller = null;
        }

        if (pedido.Articulos != null)
        {
            foreach (var item in pedido.Articulos)
            {
                if (item.Articulo != null)
                {
                    item.IdArticulo = item.Articulo.Id;
                    item.Articulo = null;
                }
            }
        }

        _context.PedidosProduccion.Add(pedido);
        
        await _context.SaveChangesAsync();
        return pedido.Id;
    }

    public async Task<PagedResult<PedidoProduccion>> GetByTallerPaginado(
        int idTaller, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
    {
        var query = QueryBase()
            .Where(p => p.Taller.Id == idTaller && p.Fecha >= desde && p.Fecha <= hasta);

        if (idEstado.HasValue && idEstado.Value > 0)
            query = query.Where(p => p.IdEstadoPedidoProduccion == idEstado.Value);

        var totalRecords = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.Fecha)
            .ThenByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<PedidoProduccion>
        {
            Items = items,
            TotalRecords = totalRecords,
            Page = page,
            PageSize = pageSize
        };
    }
    
    public async Task<List<int>> EliminarVarios(List<int> ids)
    {
        var pedidos = await _context.PedidosProduccion
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();

        _context.PedidosProduccion.RemoveRange(pedidos);
        await _context.SaveChangesAsync();
        
        return pedidos.Select(p => p.Id).ToList();
    }

    public async Task Actualizar(PedidoProduccion pedidoRecibido)
    {
        var pedidoExistente = await QueryBase()
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == pedidoRecibido.Id);

        if (pedidoExistente == null) 
            throw new NotFoundException($"El pedido {pedidoRecibido.Id} no existe.");


        if (pedidoRecibido.IdTaller == 0 && pedidoRecibido.Taller != null)
            pedidoRecibido.IdTaller = pedidoRecibido.Taller.Id;
        
        if (pedidoRecibido.IdEstadoPedidoProduccion == 0 && pedidoRecibido.EstadoPedidoProduccion != null)
            pedidoRecibido.IdEstadoPedidoProduccion = pedidoRecibido.EstadoPedidoProduccion.Id;

        pedidoRecibido.Taller = null;
        pedidoRecibido.EstadoPedidoProduccion = null;

        _context.Entry(pedidoExistente).CurrentValues.SetValues(pedidoRecibido);

        foreach (var artExistente in pedidoExistente.Articulos.ToList())
        {
            if (!pedidoRecibido.Articulos.Any(a => a.IdProduccionArticulo == artExistente.IdProduccionArticulo))
            {
                _context.Remove(artExistente);
            }
        }

        foreach (var artRecibido in pedidoRecibido.Articulos)
        {
            if (artRecibido.IdArticulo == 0 && artRecibido.Articulo != null)
            {
                artRecibido.IdArticulo = artRecibido.Articulo.Id;
            }

            var artExistente = pedidoExistente.Articulos
                .FirstOrDefault(a => a.IdProduccionArticulo == artRecibido.IdProduccionArticulo);

            if (artExistente != null && artRecibido.IdProduccionArticulo != 0)
            {
                artRecibido.Articulo = null;
                _context.Entry(artExistente).CurrentValues.SetValues(artRecibido);
            }
            else
            {
                artRecibido.IdProduccionArticulo = 0;
                artRecibido.IdPedidoProduccion = pedidoExistente.Id;
                artRecibido.Articulo = null; 
                
                pedidoExistente.Articulos.Add(artRecibido);
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<EstadoPedidoProduccion>> GetEstados()
    {
       return await _context.EstadosPedidoProduccion
            .AsNoTracking()
            .ToListAsync(); 
    }

    public async Task<PedidoProduccion> GetById(int id)
    {
        var pedido = await QueryBase()
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido!;
    }

    public async Task<List<PedidoProduccion>> GetByIds(List<int> ids)
    {
        return await QueryBase()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<bool> Existe(int id)
    {
        return await _context.PedidosProduccion.AnyAsync(p => p.Id == id);
    }

    public async Task<int> ActualizarEstadosMasivos(List<int> Ids, EstadoPedidoProduccion NuevoEstado)
    {
        if (Ids == null || !Ids.Any())
            throw new BusinessException("Debe seleccionar al menos un pedido para modificar.");

        if (NuevoEstado == null || NuevoEstado.Id <= 0)
            throw new BusinessException("El nuevo estado proporcionado no es válido.");

        return await _context.PedidosProduccion
            .Where(p => Ids.Contains(p.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(
                p => p.IdEstadoPedidoProduccion,
                NuevoEstado.Id
            ));
    }

    public async Task RestaurarCantidadPendiente(List<PedidoProduccionIngresoDetalle> detallesPPI)
    {
        if (detallesPPI == null || !detallesPPI.Any()) return;

        var idsPedidos = detallesPPI.Select(d => d.IdPedidoProduccion).Distinct().ToList();

        // 1. Traemos los pedidos
        var pedidos = await _context.PedidosProduccion
            .Where(p => idsPedidos.Contains(p.Id))
            .ToListAsync();

        // 2. Traemos TODOS los artículos de esos pedidos (necesitamos todos para saber si se completó)
        var articulosPedidos = await _context.PedidoProduccionArticulos
            .Where(pa => pa.IdPedidoProduccion.HasValue && idsPedidos.Contains(pa.IdPedidoProduccion.Value))
            .ToListAsync();

        // 3. Restauramos cantidades (Sumamos lo que estamos anulando al pendiente)
        foreach (var detalle in detallesPPI)
        {
            var artPed = articulosPedidos.FirstOrDefault(x => 
                x.IdPedidoProduccion == detalle.IdPedidoProduccion && 
                x.IdArticulo == detalle.IdArticulo);

            if (artPed != null)
            {
                artPed.CantidadPendiente += detalle.CantidadDescontada;
            }
        }

        // 4. NUEVA LÓGICA: Evaluamos el estado real del pedido
        foreach (var pedido in pedidos)
        {
            // Filtramos los artículos que pertenecen a ESTE pedido en particular
            var itemsDelPedido = articulosPedidos.Where(x => x.IdPedidoProduccion == pedido.Id).ToList();

            // Si TODOS los artículos tienen cantidad pendiente 0 o menos, está completado
            bool estaTodoCompletado = itemsDelPedido.All(a => a.CantidadPendiente <= 0);

            if (estaTodoCompletado)
            {
                pedido.IdEstadoPedidoProduccion = 1; // Completado
            }
            else
            {
                pedido.IdEstadoPedidoProduccion = 3; // En Taller / Pendiente
            }
        }
        
        // El SaveChanges lo hará el Unit of Work al final de la transacción
    }

}