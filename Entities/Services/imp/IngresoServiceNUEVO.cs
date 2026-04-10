using System.Reflection.Metadata;
using BlumeAPI.Entities;
using BlumeAPI.Repository;
using BlumeAPI.Services;

public class IngresoServiceNuevo : IIngresoService
{
    private readonly IIngresoRepository _ingresoRepository;
    private readonly IPedidoProduccionRepository _pedidoProduccionRepository;
    private readonly IPresupuestoRepository _presupuestoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IArticuloRepository _articuloRepository;
    public IngresoServiceNuevo(IIngresoRepository ingresoRepository, 
                               IPedidoProduccionRepository pedidoProduccionRepository, 
                               IPresupuestoRepository presupuestoRepository,
                               IUnitOfWork uow,
                               IArticuloRepository articuloRepository
                               )
    {
        _ingresoRepository = ingresoRepository;
        _pedidoProduccionRepository = pedidoProduccionRepository;
        _presupuestoRepository = presupuestoRepository;
        _unitOfWork = uow;
        _articuloRepository = articuloRepository;
    }

    public async Task<Ingreso> CrearIngresoConDescuentoAsync(Ingreso ingreso)
    {

        var estadosPedidos = await _pedidoProduccionRepository.GetEstados();
        var estadosPresupuestos = await _presupuestoRepository.GetEstados();
        var estadoPPTaller = estadosPedidos.FirstOrDefault(e => e.Codigo == "TA");
        var estadoPPCompleto = estadosPedidos.FirstOrDefault(e => e.Codigo == "COMP");
        var estadoPresuProduccion = estadosPresupuestos.FirstOrDefault(e => e.Codigo == "PRO");
        var estadoPresuArmado = estadosPresupuestos.FirstOrDefault(e => e.Codigo == "ARM");
        // 1. Persistir el ingreso
        var ingresoCreado = await _ingresoRepository.Crear(ingreso);
 
        // 2. Traer pedidos del taller en estado TA, ordenados de más antiguo a más reciente
        var resultadoPaginado = await _pedidoProduccionRepository
            .GetByTallerPaginado(ingreso.IdTaller, DateTime.MinValue, DateTime.MaxValue, estadoPPTaller?.Id, 1, int.MaxValue);

        var pedidosTA = resultadoPaginado.Items
            .OrderBy(p => p.Fecha)
            .ThenBy(p => p.Id)
            .ToList();

        if (!pedidosTA.Any())
            return ingresoCreado;
 
        var detalles = new List<PedidoProduccionIngresoDetalle>();
 
        // 3. Por cada artículo del ingreso, descontar pendientes FIFO
        foreach (var articuloIngreso in ingresoCreado.Articulos)
        {
            int cantidadRestante = articuloIngreso.Cantidad;
 
            foreach (var pedido in pedidosTA)
            {
                if (cantidadRestante <= 0) break;
 
                var articuloPedido = pedido.Articulos?
                    .FirstOrDefault(a => a.IdArticulo == articuloIngreso.IdArticulo);
 
                if (articuloPedido == null || articuloPedido.CantidadPendiente == 0)
                    continue;
 
                int pendienteAntes = articuloPedido.CantidadPendiente ?? 0;
                int aDescontar = Math.Min(pendienteAntes, cantidadRestante);
                int pendienteDespues = pendienteAntes - aDescontar;
 
                // Actualizar pendiente en el artículo del pedido
                articuloPedido.CantidadPendiente = pendienteDespues;
                articuloPedido.HayStock = pendienteDespues == 0;
 
                cantidadRestante -= aDescontar;
 
                detalles.Add(new PedidoProduccionIngresoDetalle
                {
                    IdPedidoProduccion = pedido.Id,
                    PedidoProduccion = pedido,
                    IdIngreso = ingresoCreado.Id,
                    Ingreso = ingresoCreado,
                    IdArticulo = articuloIngreso.IdArticulo,
                    Articulo = articuloIngreso.Articulo,
                    IdPresupuesto = pedido.IDPresupuesto, // puede ser null
                    CantidadDescontada = aDescontar,
                    CantidadPendienteAntes = pendienteAntes,
                    CantidadPendienteDespues = pendienteDespues
                });
            }
        }
 
        // 4. Actualizar estado de cada pedido afectado
        //    Si todos sus artículos quedaron en 0 → pasa a COMPLETO
        var pedidosAfectados = detalles
            .Select(d => d.PedidoProduccion)
            .DistinctBy(p => p!.Id)
            .ToList();
 
        foreach (var pedido in pedidosAfectados)
        {
            bool todosCompletos = pedido!.Articulos!
                .All(a => a.CantidadPendiente == 0);
 
            if (todosCompletos)
                pedido.IdEstadoPedidoProduccion = estadoPPCompleto?.Id ?? pedido.IdEstadoPedidoProduccion;
 
            await _pedidoProduccionRepository.Actualizar(pedido);
        }
 
        // 5. Actualizar presupuestos asociados (si existen)
        var idsPresupuestosAfectados = pedidosAfectados
            .Where(p => p!.IDPresupuesto.HasValue)
            .Select(p => p!.IDPresupuesto!.Value)
            .Distinct()
            .ToList();
 
        foreach (var idPresupuesto in idsPresupuestosAfectados)
        {
            var presupuesto = await _presupuestoRepository.GetById(idPresupuesto);
            if (presupuesto == null) continue;
 
            // Aplicar los descuentos al presupuesto
            foreach (var detalle in detalles.Where(d => d.IdPresupuesto == idPresupuesto))
            {
                var articuloPresu = presupuesto.Articulos?
                    .FirstOrDefault(a => a.IdArticulo == detalle.IdArticulo);
 
                if (articuloPresu == null) continue;
 
                articuloPresu.CantidadPendiente = detalle.CantidadPendienteDespues;
                articuloPresu.HayStock = detalle.CantidadPendienteDespues == 0;
 
                // Linkear el presupuesto al detalle
                detalle.Presupuesto = presupuesto;
            }
 
            // Estado del presupuesto según si todos los artículos tienen stock
            bool todosConStock = presupuesto.Articulos!
                .All(a => a.HayStock);
 
            presupuesto.IdEstado = todosConStock
                ? estadoPresuArmado?.Id ?? presupuesto.IdEstado
                : estadoPresuProduccion?.Id ?? presupuesto.IdEstado;
 
            await _presupuestoRepository.Actualizar(presupuesto);
        }
 
        // 6. Persistir todos los detalles de una sola vez
        await _ingresoRepository.CrearDetallesIngresoPedidoProduccion(detalles);
 
        return ingresoCreado;
    }

    public async Task ActualizarIngreso(Ingreso ingreso)
    {
        var existe = await _ingresoRepository.Existe(ingreso.Id);
        if (!existe)
            throw new NotFoundException($"Imposible actualizar: El ingreso con ID {ingreso.Id} no existe.");

        if (ingreso.Articulos.Count() <= 0)
            throw new BusinessException("La cantidad de artículos del ingreso debe ser mayor a cero.");

        await _ingresoRepository.Actualizar(ingreso);
    }
    public async Task<PagedResult<Ingreso>> GetIngresosByTaller(int idTaller, DateTime desde, DateTime hasta, EstadoIngreso? estado, int page, int pageSize)
    {
        return await _ingresoRepository.GetByTaller(idTaller, desde, hasta, estado, page, pageSize);
    }

    public async Task<Ingreso> GetById(int id)
    {
        var ingreso = await _ingresoRepository.GetById(id);
        if (ingreso == null)
            throw new NotFoundException($"No se encontró el ingreso con ID {id}.");

        return ingreso;
    }

    public async Task<List<Ingreso>> GetByIds(List<int> ids)
    {
        if (ids == null || ids.Count == 0)
            throw new BusinessException("La lista de IDs no puede estar vacía.");

        return await _ingresoRepository.GetByIds(ids);
    }

    public async Task<List<PedidoProduccionIngresoDetalle>> GetDetallesPPI(int idIngreso)
    {
        return await _ingresoRepository.GetDetallesPPI(idIngreso);
    }

    public async Task<bool> EliminarIngresoAsync(int idIngreso)
    {
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var ingreso = await _ingresoRepository.GetById(idIngreso);
            
            if (ingreso == null) {
                return false;
            }

            if (ingreso.Estado == EstadoIngreso.Eliminado) {
                return false;
            }

            var trazabilidad = await _ingresoRepository.GetDetallesPPI(idIngreso);

            await _pedidoProduccionRepository.RestaurarCantidadPendiente(trazabilidad);

            foreach (var itemIngreso in ingreso.Articulos)
            {
                var maestro = await _articuloRepository.GetByIdAsync(itemIngreso.IdArticulo);
                
                if (maestro != null)
                {
                    int stockAnterior = maestro.Stock ?? 0;
                    maestro.Stock -= itemIngreso.Cantidad;
                    

                    await _articuloRepository.Update(maestro);
                }
                else 
                {
                }
            }

            await _ingresoRepository.Eliminar(ingreso);

            int cambios = await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();
            
            return true;
        }
        catch (Exception ex)
        {

            if (ex.InnerException != null) 

            await _unitOfWork.RollbackAsync();

            throw;
        }
    }

}