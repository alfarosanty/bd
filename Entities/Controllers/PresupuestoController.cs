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
 
[HttpPost()]
    public async Task<IActionResult> Crear(Presupuesto presupuesto){
        if (presupuesto == null)
            return BadRequest("El cuerpo de la solicitud está vacío o mal formado.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            int nuevoId = await _presupuestoService.CrearPresupuesto(presupuesto);
            return Ok(new { id = nuevoId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al crear presupuesto", details = ex.Message });
        }
    }

[HttpPut()]
    public async Task<IActionResult> Actualizar([FromBody] Presupuesto presupuesto)
    {
        if (presupuesto == null)
            return BadRequest("El cuerpo de la solicitud está vacío o mal formado.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            int actualizadoId = await _presupuestoService.ActualizarPresupuesto(presupuesto);
            return Ok(new { id = actualizadoId });
        }
        catch (NotFoundException nfEx)
        {
            return NotFound(new { message = nfEx.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al actualizar presupuesto", details = ex.Message });
        }
    }


[HttpGet("GetEstadosPresupuesto")]
    public async Task<IActionResult> GetEstadosPresupuesto()
    {
        try
        {
            var estados = await _presupuestoService.GetEstadosPresupuesto();
            return Ok(estados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener estados de presupuesto", details = ex.Message });
        }
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
