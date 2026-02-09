using System.Security.Claims;
using BlumeAPI.Services;
using BlumeAPI.Services.Imp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticuloController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ArticuloController> _logger;
    private readonly IArticuloService _articuloService;

    public ArticuloController(ILogger<ArticuloController> logger, IArticuloService articuloService)
    {
        _logger = logger;
        _articuloService = articuloService;
    }

// ARTICULO

     [HttpGet("GetArticulos")]
    public IEnumerable<Articulo> GetArticulos()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().getAll(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }


 [HttpGet("BySubfamilia/{subfamilia}")]
    public IEnumerable<Articulo> GetArticulosBySubfamilia(string subfamilia)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().GetArticulosBySubfamilia(subfamilia, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }


 [HttpGet("ByFamilias/{familia}")]
    public IEnumerable<Articulo> GetArticulosByFamilia(string familia)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().GetArticuloByFamiliaMedida(familia, null, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }

    [HttpGet("ByFamiliaMedida/{familia}/{medida}")]
    public IEnumerable<Articulo> GetArticulosByFamilia(string familia, string medida)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().GetArticuloByFamiliaMedida(familia, medida, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }


[HttpPost("crearArticulos")]
public List<int> crearArticulos(Articulo[] articulos)
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    ArticuloServices articuloService = new ArticuloServices();
    List<int> idsGenerados = articuloService.crearArticulos(articulos, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);
    return idsGenerados;
}

    [HttpPost("ConsultarMedidasNecesarias")]
    public ConsultaMedida[] ConsultarMedidasNecesarias([FromBody] ArticuloPresupuesto[] articulos)
     {
         ArticuloServices articuloService = new ArticuloServices();

        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();


         var resultado = articuloService.ConsultarMedidasNecesarias(articulos, npgsqlConnection);

         return resultado.ToArray(); // convertís List<ConsultaMedida> a ConsultaMedida[]
      }

[HttpGet("cantidades-taller-corte-separado")]
public ConsultaTallerCortePorCodigo[] ConsultarCantidadesTallerCorte([FromQuery] string? codigo = null)
{
    ArticuloServices articuloService = new ArticuloServices();
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    if (!string.IsNullOrEmpty(codigo))
    {
        // Si viene código, filtramos
        var resultado = articuloService.ConsultarCantidadesTallerCorte(codigo, npgsqlConnection);
        return resultado.ToArray();
    }
    else
    {
        // Si no viene código, devolvemos todas
        var resultado = articuloService.ConsultarTodosArticulosCantidadesTallerCorte(npgsqlConnection);
        return resultado.ToArray();
    }
}


// ARTICULO PRECIO

    [HttpGet("GetArticulosPrecio")]
    public IEnumerable<ArticuloPrecio> GetArticuloPrecio()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<ArticuloPrecio> articulosPrecio = new ArticuloServices().GetArticuloPrecio(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulosPrecio;
    }


[HttpGet("ByArticuloPrecio/{articuloPrecio}")]
public IEnumerable<Articulo> GetArticulosByArticuloPrecioId(int articuloPrecio, [FromQuery] bool? habilitados = null)
{
    CConexion con = new CConexion();
    using (var npgsqlConnection = con.establecerConexion())  // <-- Mejor usar using
    {
        List<Articulo> articulos = new ArticuloServices().GetArticulosByArticuloPrecioId(articuloPrecio, habilitados ?? false, npgsqlConnection);
        // No necesitás llamar a cerrarConexion si usás "using"
        return articulos;
    }
}

    [HttpPost("CrearArticulosPrecios")]
    public List<int> CrearArticulosPrecios(ArticuloPrecio[] articuloPrecios)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<int> articulosPrecioId = new ArticuloServices().CrearArticulosPrecios(articuloPrecios, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulosPrecioId;
    }

//[Authorize]
[HttpPost("ActualizarArticulosPrecios")]
public IActionResult ActualizarArticulosPrecios([FromBody] ArticuloPrecio[] articuloPrecios)
{
    /*var claim = User.FindFirst(ClaimTypes.NameIdentifier);

    if (claim == null)
        return Unauthorized("Usuario no autenticado");

    int usuarioId = int.Parse(claim.Value);
*/
    CConexion con = new CConexion();
    NpgsqlConnection npgsqlConnection = con.establecerConexion();

    var resultado = new ArticuloServices()
        .ActualizarArticulosPrecios(articuloPrecios, npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);

    return Ok(resultado);
}


[HttpPost("ActualizarStock")]
public IActionResult ActualizarStock([FromBody] ActualizacionStockInutDTO[] articulos)
{
    try
    {
        CConexion con =  new CConexion();
        using var npgsqlConnection = con.establecerConexion();

        int cantidadAfectados = new ArticuloServices().ActualizarStock(articulos, npgsqlConnection);

        return Ok(cantidadAfectados);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Error al actualizar stock: {ex.Message}");
    }
}


[HttpGet("Presupuestados/{idArticuloPrecio}")]
public EstadisticaArticuloDTO GetArticulosPresupuestados(
    int idArticuloPrecio,
    [FromQuery] DateTime? fechaDesde,
    [FromQuery] DateTime? fechaHasta)
{
    CConexion con = new CConexion();
    using (var npgsqlConnection = con.establecerConexion())
    {
        return new ArticuloServices()
            .GetArticuloPresupuestado(idArticuloPrecio, fechaDesde, fechaHasta, npgsqlConnection);
    }
}

    [HttpGet("{id}/Facturados")]
    public async Task<IActionResult> GetFacturados(int id, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var result = await _articuloService.GetFacturadosByArticulo(id, desde, hasta);
        return Ok(result);
    }

    [HttpGet("{id}/Ingresados")]
    public async Task<IActionResult> GetIngresados(int id, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var result = await _articuloService.GetIngresadosByArticulo(id, desde, hasta);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticulo(int id)
    {
        var result = await _articuloService.GetArticulo(id);
        return Ok(result);
    }


        [HttpGet("{id}/ResumenKardex")]
    public async Task<IActionResult> GetResumenKardex(int id, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
    {
        var result = await _articuloService.GetResumenKardex(id, desde, hasta);
        return Ok(result);
    }

}