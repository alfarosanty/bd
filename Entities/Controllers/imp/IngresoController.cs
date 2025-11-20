using BlumeApi.Models;
using Entities.servicios;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class IngresoController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;
    private readonly IIngresoService iingresoService;

    public IngresoController(ILogger<ClienteController> logger, IIngresoService _ingresoService)
    {
        iingresoService = _ingresoService;
        _logger = logger;
    }

    [HttpPost("crear")]
    public async Task<int> CrearAsync(Ingreso ingreso)
    {
        int id = await iingresoService.CrearAsync(ingreso);
        return id;
    }



    [HttpPost("actualizar")]
    public async Task<int> Actualizar(Ingreso ingreso)
    {
    int id = await iingresoService.actualizarAsync(ingreso);
    return id;
    }




[HttpPost("DetallesIngresoPedidoProduccion")]
public async Task<IActionResult> CrearDetallesIngresoPedidoProduccion([FromBody] List<PedidoProduccionIngresoDetalle> detalles)
{
    try
    {
        List<int> idsCreados = await iingresoService.CrearDetallesIngresoPedidoProduccionAsync(detalles);

        return Ok(new { ids = idsCreados, mensaje = "Detalles guardados correctamente" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al guardar los detalles: {ex.Message}");
    }
}


      [HttpGet("IngresoByTaller/{idTaller}")]
    public async Task<List<Ingreso>> GetByTaller(int idTaller)
    {
        List<Ingreso> ingresos = await iingresoService.GetIngresoByTallerAsync(idTaller);
         return ingresos;
    }

    [HttpGet("IngresoByNumero/{idIngreso}")]
    public async Task<Ingreso> Get(int idIngreso)
    {
        Ingreso ingreso = await iingresoService.GetIngresoAsync(idIngreso);
        return ingreso;
    }

    [HttpGet("IngresosByIds")]
    public async Task<List<Ingreso>> GetByIds([FromQuery] string ids)
    {
        var idsIngresos = ids.Split(',').Select(int.Parse).ToList();
        List<Ingreso> ingresos = await iingresoService.GetIngresosByIdsAsync(idsIngresos);
        return ingresos;
    }

[HttpGet("DetallesIngresoPedidoProduccion/{idIngreso}")]
public async Task<List<PedidoProduccionIngresoDetalle>> GetDetallePPI(int idIngreso)
{
    List<PedidoProduccionIngresoDetalle> detallesPPI = await iingresoService.GetDetallesPPIAsync(idIngreso);
    return detallesPPI;
}

[HttpDelete("borrar")]
public async Task<IActionResult> Borrar([FromBody] Ingreso ingreso)
{
    if (ingreso == null) return BadRequest("Ingreso no puede ser null");

    try
    {
        int id =await iingresoService.BorrarIngresoAsync(ingreso);
        return Ok(new { id, mensaje = "Ingreso borrado correctamente" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al borrar el ingreso: {ex.Message}");
    }
}


}
