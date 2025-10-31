using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RemitoIngresoController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;

    public RemitoIngresoController(ILogger<ClienteController> logger)
    {
        _logger = logger;
    }
/*
    [HttpGet(Name = "GetRemitoIngreso")]
    public IEnumerable<RemitoIngreso> Get()
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        //List<Cliente> articulos = new ClienteServices().listarClientes(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return null;
    }
*/
 
    [HttpPost(Name = "CrearRemitoIngreso")]
    public void Crear(int idTaller, string descripcion, string fecha,List<ArticuloPresupuesto> ap){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        //obtener taller
        //obtener fecha

        //listar articulos y cantidades
        ArticuloPresupuesto ats = new ArticuloPresupuesto();
        List<ArticuloPresupuesto>   aingresos = new List<ArticuloPresupuesto> ();
        /**
        foreach(ArticuloPresupuesto art in ri){
        ArticuloIngreso ai = new ArticuloIngreso();
        Articulo articulo = new Articulo();
        articulo.Id = art.IdArticulo;
         //No hace falta q vaya a la BD
         //ai.articulo =  ats.GetArticulo(art.IdArticulo, npgsqlConnection);
         ai.Articulo = articulo;
         ai.cantidad = art.cantidad;
         aingresos.Add(ai);
         Console.WriteLine("Data " + art.IdArticulo);
        }   

        Presupuesto presu = new Presupuesto();
      //  string date = DateTime.UtcNow.ToString("MM-dd-yyyy");
        //TallerServices ts = new TallerServices().Get(idTaller);          
       
        new PresupuestoServices().crear(presu,npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);*/
    }

}
