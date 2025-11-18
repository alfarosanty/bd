using BlumeApi.Models;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PresupuestoController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;
    private readonly IPresupuestoService iPresupuestoService;


    public PresupuestoController(ILogger<ClienteController> logger)
    {
        _logger = logger;
    }

[HttpGet("GetPresupuestoByNumero/{idPresupuesto}")]
public IActionResult Get(int idPresupuesto)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
    PresupuestoServices ps = new PresupuestoServices();

    try
    {
        Presupuesto presu = iPresupuestoService.GetPresupuesto(idPresupuesto);
        return Ok(presu); // HTTP 200 con datos
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("No se encontró presupuesto"))
            return NotFound(new { mensaje = ex.Message }); // HTTP 404
        else
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
    }
}


 
    [HttpPost("crear")]
    public int  Crear(Presupuesto presupuesto){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        PresupuestoServices  ps = new PresupuestoServices();
        int id =  ps.crear(presupuesto, npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
        return id;  
    }

[HttpPost("actualizar")]
public IActionResult Actualizar([FromBody] Presupuesto presupuesto)
{
    if (presupuesto == null)
        return BadRequest("El cuerpo de la solicitud está vacío o mal formado.");

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = null;

    try
    {
        npgsqlConnection = con.establecerConexion();
        PresupuestoServices ps = new PresupuestoServices();
        int id = ps.actualizar(presupuesto, npgsqlConnection);
        return Ok( id );
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Error al actualizar presupuesto", details = ex.Message });
    }
    finally
    {
        if (npgsqlConnection != null && npgsqlConnection.State == System.Data.ConnectionState.Open)
        {
            con.cerrarConexion(npgsqlConnection);
        }
    }
}



      [HttpGet("GetPresupuestoByCliente/{idCliente}")]
    public List<Presupuesto> GetByCliente(int idCliente)
    {
         CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
       PresupuestoServices  ps = new PresupuestoServices();
        List<Presupuesto> presu = ps.GetPresupuestoByCliente(idCliente,npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
         return presu;
    }

[HttpGet("GetEstadosPresupuesto")]
public IActionResult GetEstadosPresupuesto()
{
    try
    {
        CConexion con = new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        PresupuestoServices ps = new PresupuestoServices();
        List<EstadoPresupuesto> estadosPresupuesto = ps.getEstadosPresupuesto(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);

        return Ok(estadosPresupuesto);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { mensaje = "Error al obtener estados de presupuesto.", detalle = ex.Message });
    }
}


[HttpGet("ArticulosPresupuestados")]
public ActionResult<List<ArticuloPresupuesto>> articulosPresupuestados([FromQuery] int idArticuloPrecio, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    PresupuestoServices ps = new PresupuestoServices();

    // Llamás al servicio pasando las fechas recibidas
    List<ArticuloPresupuesto> listaDeArticulosPresupuestados = ps.articulosPresupuestados(idArticuloPrecio, fechaInicio, fechaFin, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);

    return listaDeArticulosPresupuestados;
}

    [HttpGet("PresupuestosByIds")]
    public List<Presupuesto> GetByIds([FromQuery] string ids)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        PresupuestoServices  presupuestoService = new PresupuestoServices();
        var idsPresupuestos = ids.Split(',').Select(int.Parse).ToList();
        List<Presupuesto> presupuestos = presupuestoService.GetPresupuestosByIds(idsPresupuestos,npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return presupuestos;
    }

}
