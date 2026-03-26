using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PresupuestoController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;
    private readonly IPresupuestoService _presupuestoService;

    public PresupuestoController(ILogger<ClienteController> logger, IPresupuestoService iPresupuestoService)
    {
        _presupuestoService = iPresupuestoService;
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
        Presupuesto presu = ps.GetPresupuesto(idPresupuesto, npgsqlConnection);
        return Ok(presu); // HTTP 200 con datos
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("No se encontró presupuesto"))
            return NotFound(new { mensaje = ex.Message }); // HTTP 404
        else
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
    }
    finally
    {
        con.cerrarConexion(npgsqlConnection); // Aseguramos cierre de conexión
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
public IActionResult GetByCliente(
    int idCliente,
    [FromQuery] DateTime? desde,
    [FromQuery] DateTime? hasta)
{
    if (idCliente <= 0)
        return BadRequest("Id de cliente inválido");

    var con = new CConexion();
    NpgsqlConnection npgsqlConnection = null;

    try
    {
        npgsqlConnection = con.establecerConexion();

        var ps = new PresupuestoServices();
        var presu = ps.GetPresupuestoByCliente(
            idCliente,
            desde,
            hasta,
            npgsqlConnection
        );

        if (presu == null || presu.Count == 0)
            return NoContent(); // 204

        return Ok(presu); // 200
    }
    catch (PostgresException ex)
    {
        return StatusCode(500, "Error de la Base de datos: " + ex.Message);
    }

    catch (Exception ex)
    {
        // error inesperado
        return StatusCode(500, "Error interno del servidor");
    }
    finally
    {
        if (npgsqlConnection != null)
            con.cerrarConexion(npgsqlConnection);
    }
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

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] int? idEstado,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest("Los parámetros de paginación deben ser mayores a 0.");

        var result = await _presupuestoService.GetPresupuestosAsync(desde, hasta, idEstado, page, pageSize);

        if (result == null || result.Items.Count == 0)
            return NoContent();

        return Ok(result);
    }

    [HttpGet("cliente/{idCliente}")]
    public async Task<IActionResult> GetByCliente(
        [FromRoute] int idCliente,
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        [FromQuery] int? idEstado,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        var result = await _presupuestoService.GetPresupuestosByClienteAsync(idCliente, desde, hasta, idEstado, page, pageSize);

        if (result == null || result.Items.Count == 0)
            return NoContent();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _presupuestoService.GetPresupuestoByIdAsync(id);

        if (result == null)
            return NotFound(new { mensaje = $"No se encontró el presupuesto con id {id}" });

        return Ok(result);
    }

}
