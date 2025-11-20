using BlumeApi.Models;
using BlumeAPI.servicios;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PedidoProduccionController : ControllerBase
{

    private readonly ILogger<PedidoProduccionController> _logger;
    private readonly IPedidoProduccionService iPedidoProduccionService;

    public PedidoProduccionController(ILogger<PedidoProduccionController> logger, IPedidoProduccionService _pedidoProduccionService)
    {
        _logger = logger;
        iPedidoProduccionService = _pedidoProduccionService;
    }
 
    [HttpPost("crear")]
    public async Task<int>  Crear(PedidoProduccion pedidoProduccion){
        int id =  await iPedidoProduccionService.CrearAsync(pedidoProduccion);
        return id;  
    }



    [HttpPost("actualizar")]
    public async Task<int> Actualizar(PedidoProduccion pedidoProduccion)
    {
    int id = await iPedidoProduccionService.ActualizarAsync(pedidoProduccion);
    return id;
    }

    [HttpPost("ObtenerClientes")]
    public async Task<List<ClienteXPedidoProduccionOutputDTO>> ObtenerClientes([FromBody] List<int> idPedidos)
    {
        var clientesXPedidoProduccion = await iPedidoProduccionService.obtenerClientesAsync(idPedidos);

        return clientesXPedidoProduccion;
    }

[HttpPost("EliminarPedidosProduccion")]
public async Task<ActionResult<List<int>>> EliminarPedidosProduccion([FromBody] List<int> idPedidos)
{
    if (idPedidos == null || idPedidos.Count == 0)
        return BadRequest("No se enviaron IDs para eliminar.");

    try
    {
        
        var idsEliminados = await iPedidoProduccionService.eliminarPedidosProduccion(idPedidos);

        if (idsEliminados.Count == 0)
            return NotFound("No se encontraron pedidos para eliminar con los IDs enviados.");

        return Ok(idsEliminados);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error al eliminar pedidos: {ex}");
        return StatusCode(StatusCodes.Status500InternalServerError,
            $"Ocurrió un error al eliminar los pedidos: {ex.Message}");
    }
}



      [HttpGet("GetPedidoProduccionByTaller/{idTaller}")]
    public async Task<List<PedidoProduccion>> GetByTaller(int idTaller)
    {
        List<PedidoProduccion> pedidoProducciones = await iPedidoProduccionService.GetPedidoProduccionByTaller(idTaller);
         return pedidoProducciones;
    }

         [HttpGet("GetPedidoProduccionByNumero/{idPedidoProduccion}")]
    public async Task<PedidoProduccion> Get(int idPedidoProduccion)
    {
        PedidoProduccion pedidoProduccion = await iPedidoProduccionService.GetPedidoProduccionAsync(idPedidoProduccion);
        return pedidoProduccion;
    }


         [HttpGet("GetEstadosPedidoProduccion")]
    public async Task<List<EstadoPedidoProduccion>> GetEstadosPedidoProduccion()
    {
        List<EstadoPedidoProduccion> estadosPedidoProduccion = await iPedidoProduccionService.GetEstadosPedidoProduccionAsync();
        return estadosPedidoProduccion;
    }


    [HttpPatch("ActualizarEstadosPedidoProduccion")]
    public async Task<List<int>> ActualizarEstadosPedidoProduccion([FromBody] List<PedidoProduccionEstadoDTO> lista)
    {

        List<int> idsPedidosProducionActualizados = await iPedidoProduccionService.actualizarEstadosPedidoProduccionAsync(lista);

        return idsPedidosProducionActualizados;
    }

    [HttpGet("PedidosProduccionByIds")]
    public async Task<List<PedidoProduccion>> GetByIds([FromQuery] string ids)
    {
        var idsPedidosProduccion = ids.Split(',').Select(int.Parse).ToList();
        List<PedidoProduccion> pedidosProduccion = await iPedidoProduccionService.GetPedidosProduccionByIdsAsync(idsPedidosProduccion);
        return pedidosProduccion;
    }


}
