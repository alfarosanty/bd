using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PedidoProduccionController : ControllerBase
{

    private readonly ILogger<PedidoProduccionController> _logger;

    public PedidoProduccionController(ILogger<PedidoProduccionController> logger)
    {
        _logger = logger;
    }
 
    [HttpPost("crear")]
    public int  Crear(PedidoProduccion pedidoProduccion){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        PedidoProduccionService  pps = new PedidoProduccionService();
        int id =  pps.crear(pedidoProduccion, npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
        return id;  
    }



    [HttpPost("actualizar")]
    public int Actualizar(PedidoProduccion pedidoProduccion)
    {
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    PedidoProduccionService pps = new PedidoProduccionService();
    int id = pps.actualizar(pedidoProduccion, npgsqlConnection);
    con.cerrarConexion(npgsqlConnection);
    return id;
    }

    [HttpPost("ObtenerClientes")]
    public List<ClienteXPedidoProduccionOutputDTO> ObtenerClientes([FromBody] List<int> idPedidos)
    {
        using var conexion = new CConexion().establecerConexion();

        PedidoProduccionService pps = new PedidoProduccionService();
        var clientesXPedidoProduccion = pps.obtenerClientes(conexion, idPedidos);

        return clientesXPedidoProduccion;
    }

[HttpPost("EliminarPedidosProduccion")]
public ActionResult<List<int>> EliminarPedidosProduccion([FromBody] List<int> idPedidos)
{
    if (idPedidos == null || idPedidos.Count == 0)
        return BadRequest("No se enviaron IDs para eliminar.");

    try
    {
        using var conexion = new CConexion().establecerConexion();

        PedidoProduccionService pps = new PedidoProduccionService();
        var idsEliminados = pps.eliminarPedidosProduccion(conexion, idPedidos);

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
         CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
       PedidoProduccionService  pps = new PedidoProduccionService();
        List<PedidoProduccion> pedidoProducciones = pps.GetPedidoProduccionByTaller(idTaller,npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
         return pedidoProducciones;
    }

         [HttpGet("GetPedidoProduccionByNumero/{idPedidoProduccion}")]
    public PedidoProduccion Get(int idPedidoProduccion)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        PedidoProduccionService  pps = new PedidoProduccionService();
        PedidoProduccion pedidoProduccion = pps.getPedidoProduccion(idPedidoProduccion,npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return pedidoProduccion;
    }


         [HttpGet("GetEstadosPedidoProduccion")]
    public List<EstadoPedidoProduccion> GetEstadosPedidoProduccion()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        PedidoProduccionService  pps = new PedidoProduccionService();
        List<EstadoPedidoProduccion> estadosPedidoProduccion = pps.getEstadosPedidoProduccion(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return estadosPedidoProduccion;
    }


    [HttpPatch("ActualizarEstadosPedidoProduccion")]
    public List<int> ActualizarEstadosPedidoProduccion([FromBody] List<PedidoProduccionEstadoDTO> lista)
    {
        CConexion con = new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        PedidoProduccionService pps = new PedidoProduccionService();
        List<int> idsPedidosProducionActualizados = pps.actualizarEstadosPedidoProduccion(npgsqlConnection, lista);

        con.cerrarConexion(npgsqlConnection);
        return idsPedidosProducionActualizados;
    }

    [HttpGet("PedidosProduccionByIds")]
    public List<PedidoProduccion> GetByIds([FromQuery] string ids)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        PedidoProduccionService  pedidoProduccionService = new PedidoProduccionService();
        var idsPedidosProduccion = ids.Split(',').Select(int.Parse).ToList();
        List<PedidoProduccion> pedidosProduccion = pedidoProduccionService.GetPedidosProduccionByIds(idsPedidosProduccion,npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return pedidosProduccion;
    }


}
