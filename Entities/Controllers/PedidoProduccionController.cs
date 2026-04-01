using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PedidoProduccionController : ControllerBase
{

    private readonly ILogger<PedidoProduccionController> _logger;
    private readonly IPedidoProduccionService _pedidoProduccionService;

    public PedidoProduccionController(ILogger<PedidoProduccionController> logger, IPedidoProduccionService pedidoProduccionService)
    {
        _logger = logger;
        _pedidoProduccionService = pedidoProduccionService;
    }
 
[HttpPost("crear")]
    public async Task<ActionResult<int>> Crear(PedidoProduccion pedido)
    {
        var id = await _pedidoProduccionService.CrearPedido(pedido);
        return Ok(id);
    }

[HttpGet("taller/{idTaller}")]
    public async Task<IActionResult> GetByTaller(
        [FromRoute] int idTaller,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] int? idEstado,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        var result = await _pedidoProduccionService.ListarPorTallerPaginado(
            idTaller, desde, hasta, idEstado, page, pageSize);

        if (result == null || result.Items.Count == 0)
            return NoContent();

        return Ok(result);
    }

[HttpPost("eliminar")]
    public async Task<ActionResult<List<int>>> Eliminar([FromBody] List<int> idPedidos)
    {
        var eliminados = await _pedidoProduccionService.EliminarPedidos(idPedidos);
        if (!eliminados.Any()) return NotFound("No se eliminó nada.");
        return Ok(eliminados);
    }



[HttpPut("{id}")]
    public async Task<ActionResult<int>> Actualizar(int id, [FromBody] PedidoProduccion pedidoProduccion)
    {
        if (id != pedidoProduccion.Id) return BadRequest("ID de ruta no coincide con el ID del objeto.");
        
        await _pedidoProduccionService.Actualizar(pedidoProduccion);
        return Ok(id);
    }

[HttpGet("{id}")]
    public async Task<ActionResult<PedidoProduccion>> Get(int id)
    {
        if (id <= 0)
        {
            throw new BusinessException("El ID proporcionado no es válido. Debe ser mayor a cero.");
        }

        var pedido = await _pedidoProduccionService.GetById(id);

        if (pedido == null)
        {
            throw new NotFoundException($"No se encontró el pedido de producción con ID: {id}");
        }

        return Ok(pedido);
    }


[HttpGet("estados")]
    public async Task<ActionResult<List<EstadoPedidoProduccion>>> GetEstadosPedidoProduccion()
    {
        var estados = await _pedidoProduccionService.ListarEstados();

        if (estados == null || !estados.Any())
        {
            throw new NotFoundException("No se encontraron estados de producción registrados.");
        }

        return Ok(estados);
    }


[HttpPatch("actualizar-estados")]
    public async Task<ActionResult> ActualizarEstadosMasivos([FromBody] ActualizacionEstadosDTO dto)
    {
        if (dto.PedidoIds == null || !dto.PedidoIds.Any())
            throw new BusinessException("Debe proporcionar al menos un ID de pedido.");

        await _pedidoProduccionService.ActualizarEstadoVarios(dto);
        
        return Ok(new { Mensaje = $"Se procesó la actualización de {dto.PedidoIds.Count} pedidos." });
    }

[HttpPost("ByIds")]
    public async Task<ActionResult<List<PedidoProduccion>>> GetPedidosProduccionByIds([FromBody] List<int> ids)
    {
        if (ids == null || !ids.Any())
            throw new BusinessException("Debe proporcionar al menos un ID.");

        var pedidos = await _pedidoProduccionService.GetByIds(ids);
        return Ok(pedidos);
    }


}
