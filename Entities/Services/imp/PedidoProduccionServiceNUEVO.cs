
using BlumeAPI.Entities.Repository;
using BlumeAPI.Repository;
using BlumeAPI.Services;

public class PedidoProduccionServiceNUEVO : IPedidoProduccionService
{
    private readonly IPedidoProduccionRepository _pedidoProduccionRepository;
    private readonly IPresupuestoRepository _presupuestoRepository;

    public PedidoProduccionServiceNUEVO(IPedidoProduccionRepository pedidoProduccionRepository, IPresupuestoRepository presupuestoRepository)
    {
        _pedidoProduccionRepository = pedidoProduccionRepository;
        _presupuestoRepository = presupuestoRepository;
    }

    public async Task<int> CrearPedido(PedidoProduccion pedido)
    {
        if (pedido.Fecha == DateTime.MinValue) pedido.Fecha = DateTime.Now;
        
        return await _pedidoProduccionRepository.Crear(pedido);
    }

    public async Task<PagedResult<PedidoProduccionDTO>> ListarPorTallerPaginado(
        int idTaller, DateTime desde, DateTime hasta, int? idEstado, int page, int pageSize)
    {
        var resultadoPaginado = await _pedidoProduccionRepository.GetByTallerPaginado(idTaller, desde, hasta, idEstado, page, pageSize);

        var idsPresupuestos = resultadoPaginado.Items
            .Where(p => p.IDPresupuesto.HasValue)
            .Select(p => p.IDPresupuesto!.Value)
            .Distinct()
            .ToList();

        var mapaClientes = idsPresupuestos.Any() 
            ? await _presupuestoRepository.GetNombresClientesPorPresupuestos(idsPresupuestos)
            : new Dictionary<int, string>();

        var dtos = resultadoPaginado.Items.Select(p => new PedidoProduccionDTO
        {
            Id = p.Id,
            Fecha = p.Fecha,
            Taller = p.Taller,
            EstadoPedidoProduccion = p.EstadoPedidoProduccion,
            IdPresupuesto = p.IDPresupuesto,
            Articulos = p.Articulos,
            ClienteNombre = p.IDPresupuesto.HasValue && mapaClientes.TryGetValue(p.IDPresupuesto.Value, out var nombre)
                            ? nombre 
                            : "Stock"
        }).ToList();

        return new PagedResult<PedidoProduccionDTO>
        {
            Items = dtos,
            TotalRecords = resultadoPaginado.TotalRecords,
            Page = resultadoPaginado.Page,
            PageSize = resultadoPaginado.PageSize
        };
    }

    public async Task<List<int>> EliminarPedidos(List<int> ids)
    {
        return await _pedidoProduccionRepository.EliminarVarios(ids);
    }

    public async Task<List<EstadoPedidoProduccion>> ListarEstados()
    {
        return await _pedidoProduccionRepository.GetEstados();
    }

    public async Task<PedidoProduccion> GetById(int id)
    {
        var pedido = await _pedidoProduccionRepository.GetById(id);
        return pedido;
    }

    public async Task Actualizar(PedidoProduccion pedido)
    {
        var existe = await _pedidoProduccionRepository.Existe(pedido.Id);
        if (!existe)
            throw new NotFoundException($"Imposible actualizar: El pedido con ID {pedido.Id} no existe.");

        if (pedido.Articulos.Count() <= 0)
            throw new BusinessException("La cantidad de artículos del pedido debe ser mayor a cero.");

        await _pedidoProduccionRepository.Actualizar(pedido);
    }

    public async Task ActualizarEstadoVarios(ActualizacionEstadosDTO dto)
    {
        int actualizados = await _pedidoProduccionRepository.ActualizarEstadosMasivos(dto.PedidoIds, dto.NuevoEstado);

        if (actualizados == 0)
            throw new BusinessException("No se pudo actualizar ningún pedido. Verifique los IDs proporcionados.");
    }

    public async Task<List<PedidoProduccion>> GetByIds(List<int> ids)
    {
        return await _pedidoProduccionRepository.GetByIds(ids);

    }
}