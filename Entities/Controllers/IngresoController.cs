using System.Threading.Tasks;
using BlumeAPI.Entities;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class IngresoController : ControllerBase
{

private readonly ILogger<ClienteController> _logger;
private readonly IIngresoService _ingresoService;

public IngresoController(ILogger<ClienteController> logger, IIngresoService ingresoService)
{
    _logger = logger;
    _ingresoService = ingresoService;
}
 
    [HttpPost()]
public async Task<Ingreso>  Crear(Ingreso ingreso){
             
    return await _ingresoService.CrearIngresoConDescuentoAsync(ingreso);  
}



    [HttpPut()]
public async Task<IActionResult> Actualizar(Ingreso ingreso)
{
    await _ingresoService.ActualizarIngreso(ingreso);
    return Ok(ingreso.Id);
}

    [HttpGet("taller/{idTaller}")]
public async Task<IActionResult> GetByTaller(
    [FromRoute] int idTaller,
    [FromQuery] DateTime desde,
    [FromQuery] DateTime hasta,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 15)
{
    return Ok(await _ingresoService.GetIngresosByTaller(idTaller,desde, hasta, page, pageSize));
}

[HttpGet("{idIngreso}")]
public async Task<IActionResult> Get(int idIngreso)
{
    var ingreso = await _ingresoService.GetById(idIngreso);
    return Ok(ingreso);
}

[HttpPost("ByIds")]
    public async Task<IActionResult> GetByIds([FromBody] List<int> ids)
    {
        var ingresos = await _ingresoService.GetByIds(ids);
        return Ok(ingresos);
    }

[HttpGet("DetallesIngresoPedidoProduccion/{idIngreso}")]
    public async Task<IActionResult> GetDetallePPI(int idIngreso)
    {
        var detallesPPI = await _ingresoService.GetDetallesPPI(idIngreso);
        return Ok(detallesPPI);
    }

[HttpDelete()]
    public async Task<IActionResult> Borrar([FromBody] Ingreso ingreso)
    {
        var eliminados = await _ingresoService.EliminarIngresos(new List<int> { ingreso.Id });
        if (!eliminados.Any()) return NotFound("No se eliminó nada.");
        return Ok(eliminados);
    }
}

