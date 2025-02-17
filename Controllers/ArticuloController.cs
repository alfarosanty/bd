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
    public IEnumerable<Articulo> Get()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Articulo> articulos = new ArticuloServices().listarArticulos(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
    }

}
