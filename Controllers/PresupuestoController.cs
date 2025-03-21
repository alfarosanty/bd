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
 
    [HttpPost("crear")]
    public int  Crear(Presupuesto presupuesto){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        PresupuestoServices  ps = new PresupuestoServices();
        int id =  ps.crear(presupuesto, npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
        return id;  
    }

    [HttpPost("actualizar")]
    public int Actualizar(Presupuesto presupuesto)
    {
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    PresupuestoServices ps = new PresupuestoServices();
    int id = ps.actualizar(presupuesto, npgsqlConnection);
    con.cerrarConexion(npgsqlConnection);
    return id;
    }


      [HttpGet("GetPresupuestoByCliente/{idCliente}")]
    public List<Presupuesto> GetByCliente(int idCliente)
    {
         CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
       PresupuestoServices  ps = new PresupuestoServices();
        List<Presupuesto> presu = ps.GetPresupuestoByCliente(idCliente,npgsqlConnection);
         con.cerrarConexion(npgsqlConnection);
         return presu;
    }

}
