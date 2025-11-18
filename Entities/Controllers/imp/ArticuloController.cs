using Microsoft.AspNetCore.Mvc;
using BlumeAPI.Services;
using BlumeAPI.Models;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticuloController : ControllerBase
{
    private readonly IArticuloService iarticuloService;

    public ArticuloController(IArticuloService service)
    {
        iarticuloService = service;
    }

    [HttpGet("GetArticulos")]
    public async Task<ActionResult<IEnumerable<Articulo>>> GetArticulos()
        => Ok(await iarticuloService.GetAllAsync());

    [HttpPost("crearArticulos")]
    public async Task<ActionResult<List<int>>> CrearArticulos([FromBody] List<Articulo> articulos)
        => Ok(await iarticuloService.CrearArticulosAsync(articulos));

[HttpPost("ConsultarMedidasNecesarias")]
public async Task<ActionResult<ConsultaMedida[]>> ConsultarMedidasNecesarias([FromBody] ArticuloPresupuesto[] articulos)
{
    var resultado = await iarticuloService.ConsultarMedidasNecesarias(articulos);
    return Ok(resultado.ToArray());
}


[HttpGet("cantidades-taller-corte-separado")]
public async Task<ActionResult<ConsultaTallerCortePorCodigo[]>> ConsultarCantidadesTallerCorte([FromQuery] string? codigo = null)
{
    if (!string.IsNullOrEmpty(codigo))
    {
        var resultado = await iarticuloService.ConsultarCantidadesTallerCorte(codigo);
        return Ok(resultado.ToArray());
    }
    else
    {
        var resultado = await iarticuloService.ConsultarTodosArticulosCantidadesTallerCorte();
        return Ok(resultado.ToArray());
    }
}





// ======================= ARTICULOS PRECIO =======================

    [HttpGet("GetArticulosPrecio")]
    public async Task<ActionResult<IEnumerable<ArticuloPrecio>>> GetArticulosPrecio()
        => Ok(await iarticuloService.GetArticulosPrecioAsync());

    [HttpGet("ByArticuloPrecio/{articuloPrecio}")]
    public async Task<ActionResult<IEnumerable<Articulo>>> GetByArticuloPrecioId(int articuloPrecio, [FromQuery] bool? habilitados = null)
        => Ok(await iarticuloService.GetArticulosByArticuloPrecioId(articuloPrecio, habilitados ?? false));

    [HttpPost("CrearArticulosPrecios")]
    public async Task<ActionResult<List<int>>> CrearArticulosPrecios([FromBody] ArticuloPrecio[] articuloPrecios)
        => Ok(await iarticuloService.CrearArticulosPreciosAsync(articuloPrecios));

    [HttpPost("ActualizarArticulosPrecios")]
    public async Task<ActionResult<List<int>>> ActualizarArticulosPrecios([FromBody] ArticuloPrecio[] articuloPrecios)
        => Ok(await iarticuloService.ActualizarArticulosPreciosAsync(articuloPrecios));

    [HttpPost("ActualizarStock")]
    public async Task<ActionResult<int>> ActualizarStock([FromBody] ActualizacionStockInutDTO[] articulos)
        => Ok(await iarticuloService.ActualizarStockAsync(articulos));

    [HttpGet("Presupuestados/{idArticuloPrecio}")]
    public async Task<ActionResult<EstadisticaArticuloDTO>> GetArticulosPresupuestados(
        int idArticuloPrecio, [FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta)
        => Ok(await iarticuloService.GetArticuloPresupuestadoAsync(idArticuloPrecio, fechaDesde, fechaHasta));
}
