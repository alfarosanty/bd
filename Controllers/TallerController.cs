using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TallerController : ControllerBase
{

    private readonly ILogger<TallerController> _logger;

    public TallerController(ILogger<TallerController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetTalleres")]
    public IEnumerable<Taller> Get()
    {
        
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Taller> talleres = new TallerServices().listarTalleres(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return talleres;
    }


}
