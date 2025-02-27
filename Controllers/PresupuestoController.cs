using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PresupuestoController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;

    public PresupuestoController(ILogger<ClienteController> logger)
    {
        _logger = logger;
    }

     [HttpGet("GetPresupuestoByNumero/{idPresupuesto}")]
    public Presupuesto Get(int idPresupuesto)
    {
         CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
       PresupuestoServices  ps = new PresupuestoServices();
        Presupuesto presu = ps.GetPresupuesto(idPresupuesto,npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
         return presu;
    }
 
    [HttpPost()]
    public int  Crear(Presupuesto presupuesto){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        PresupuestoServices  ps = new PresupuestoServices();
       int id =  ps.crear(presupuesto, npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
         return id;
    }

}
