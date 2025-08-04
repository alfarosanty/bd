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


      [HttpGet("GetPedidoProduccionByTaller/{idTaller}")]
    public List<PedidoProduccion> GetByTaller(int idTaller)
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
}
