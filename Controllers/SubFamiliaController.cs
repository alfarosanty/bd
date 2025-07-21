using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class SubFamiliaController : ControllerBase{

    private readonly ILogger<SubFamiliaController> _logger;

    public SubFamiliaController(ILogger<SubFamiliaController> logger)
    {
        _logger = logger;
    }


    [HttpGet("GetSubFamilias")]
    public IEnumerable<SubFamilia> Get()
    {
        
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<SubFamilia> subFamilias = new SubFamiliaServices().listarSubFamilias(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return subFamilias;
    }



}