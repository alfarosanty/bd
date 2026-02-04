using BlumeAPI.Entities.clases.modelo;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class MedidaController : ControllerBase{

    private readonly ILogger<MedidaController> _logger;

    public MedidaController(ILogger<MedidaController> logger)
    {
        _logger = logger;
    }


    [HttpGet("GetMedidas")]
    public IEnumerable<Medida> Get()
    {
        
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Medida> medidas = new MedidaServices().listarMedidas(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return medidas;
    }



}