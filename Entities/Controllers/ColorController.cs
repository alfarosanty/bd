using BlumeAPI.Entities.clases.modelo;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class ColorController : ControllerBase{

    private readonly ILogger<ColorController> _logger;

    public ColorController(ILogger<ColorController> logger)
    {
        _logger = logger;
    }


    [HttpGet("GetColores")]
    public IEnumerable<Color> Get()
    {
        
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Color> colores = new ColorServices().listarColores(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return colores;
    }



}