using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class IngresoController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;

    public IngresoController(ILogger<ClienteController> logger)
    {
        _logger = logger;
    }
 
    [HttpPost("crear")]
    public int  Crear(Ingreso ingreso){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        IngresoService  ingresoService = new IngresoService();
        int id =  ingresoService.crear(ingreso, npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
        return id;  
    }



    [HttpPost("actualizar")]
    public int Actualizar(Ingreso ingreso)
    {
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        IngresoService  ingresoService = new IngresoService();
    int id = ingresoService.actualizar(ingreso, npgsqlConnection);
    con.cerrarConexion(npgsqlConnection);
    return id;
    }


        // POST api/ingresodetalle
[HttpPost("DetallesIngresoPedidoProduccion")]
public IActionResult CrearDetallesIngresoPedidoProduccion([FromBody] List<PedidoProduccionIngresoDetalle> detalles)
{
    try
    {
        CConexion con = new CConexion();
        using var npgsqlConnection = con.establecerConexion();
        using var transaction = npgsqlConnection.BeginTransaction();

        IngresoService ingresoService = new IngresoService();
        // Guardamos los IDs creados
        List<int> idsCreados = ingresoService.CrearDetallesIngresoPedidoProduccion(detalles, npgsqlConnection);

        transaction.Commit();

        return Ok(new { ids = idsCreados, mensaje = "Detalles guardados correctamente" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al guardar los detalles: {ex.Message}");
    }
}


      [HttpGet("GetIngresoByTaller/{idTaller}")]
    public List<Ingreso> GetByTaller(int idTaller)
    {
         CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        IngresoService  ingresoService = new IngresoService();
        List<Ingreso> ingresos = ingresoService.GetIngresoByTaller(idTaller,npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
         return ingresos;
    }

    [HttpGet("GetIngresoByNumero/{idIngreso}")]
    public Ingreso Get(int idIngreso)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        IngresoService  ingresoService = new IngresoService();
        Ingreso ingreso = ingresoService.getIngreso(idIngreso,npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return ingreso;
    }



}
