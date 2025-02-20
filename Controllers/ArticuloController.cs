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

     [HttpGet(Name = "GetArticulos")]
    public IEnumerable<Articulo> GetArticulos()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().listarArticulos(npgsqlConnection);
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


}
