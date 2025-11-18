using BlumeApi.Models;
using BlumeAPI.Models;
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
public async Task<IActionResult> Get(int idPresupuesto)
{
    try
    {
        Presupuesto? presu = await iPresupuestoService.GetPresupuesto(idPresupuesto);
        return Ok(presu);
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("No se encontró presupuesto"))
            return NotFound(new { mensaje = ex.Message });
        else
            return StatusCode(500, new { mensaje = "Error interno del servidor." });
    }
}


 
    [HttpPost("crear")]
        public async Task<IActionResult> Crear(Presupuesto presupuesto)
    {

        int id = await iPresupuestoService.CrearPresupuestoAsync(presupuesto);

        return Ok(id);
    }

[HttpPost("actualizar")]
public async Task<IActionResult> Actualizar([FromBody] Presupuesto presupuesto)
{
    if (presupuesto == null)
        return BadRequest("El cuerpo de la solicitud está vacío o mal formado.");

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    try
    {
        bool ok = await iPresupuestoService.ActualizarPresupuestoAsync(presupuesto);

        if (!ok)
            return NotFound("No se encontró el presupuesto a actualizar.");

        return Ok(new { message = "Presupuesto actualizado correctamente", id = presupuesto.Id });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new
        {
            message = "Error al actualizar el presupuesto",
            details = ex.Message
        });
    }
}




      [HttpGet("GetPresupuestoByCliente/{idCliente}")]
    public async Task<List<Presupuesto>> GetPresupuestosByCliente(int idCliente)
    {
        List<Presupuesto> presus = await iPresupuestoService.GetPresupuestoByCliente(idCliente);
         return presus;
    }

[HttpGet("GetEstadosPresupuesto")]
public async Task<IActionResult> GetEstadosPresupuesto()
{
    try
    {
        List<EstadoPresupuesto> estadosPresupuesto = await iPresupuestoService.getEstadosPresupuesto();

        return Ok(estadosPresupuesto);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { mensaje = "Error al obtener estados de presupuesto.", detalle = ex.Message });
    }
}


[HttpGet("ArticulosPresupuestados")]
public async Task< ActionResult<List<ArticuloPresupuesto>>> articulosPresupuestados([FromQuery] int idArticuloPrecio, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
{

    List<ArticuloPresupuesto> listaDeArticulosPresupuestados = await iPresupuestoService.articulosPresupuestados(idArticuloPrecio, fechaInicio, fechaFin);


    return listaDeArticulosPresupuestados;
}

    [HttpGet("PresupuestosByIds")]
    public async Task< List<Presupuesto>> GetByIds([FromQuery] string ids)
    {
        var idsPresupuestos = ids.Split(',').Select(int.Parse).ToList();
        List<Presupuesto> presupuestos = await iPresupuestoService.GetPresupuestosByIds(idsPresupuestos);
        return presupuestos;
    }

}
