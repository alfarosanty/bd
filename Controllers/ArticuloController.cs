using Microsoft.AspNetCore.Mvc;

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

    public ArticuloController(ILogger<ArticuloController> logger)
    {
        _logger = logger;
    }

     [HttpGet("GetArticulos")]
    public IEnumerable<Articulo> GetArticulos()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().getAll(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }



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
    public void crearArticulos(Articulo[] articulos)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        ArticuloServices  articuloService = new ArticuloServices();
        articuloService.crearArticulos(articulos, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        }


}
