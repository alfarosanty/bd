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
 
    [HttpPost(Name = "CrearPresupuesto")]
    public void Crear(int idClinete, string fecha,List<ArticuloPresupuesto> ri){
        /**CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        Cliente cl = mew Cliente();
        cl.Id = idClinete;
       
        List<ArticuloPresupuesto>   aingresos = new List<ArticuloPresupuesto> ();
        foreach(ArticuloCantidad art in ri){
            ArticuloPresupuesto ai = new ArticuloPresupuesto();
            Articulo articulo = new Articulo();
            articulo.Id = art.IdArticulo;
            //No hace falta q vaya a la BD
            //ai.articulo =  ats.GetArticulo(art.IdArticulo, npgsqlConnection);
            ai.Articulo = articulo;
            ai.cantidad = art.cantidad;
            aingresos.Add(ai);
            Console.WriteLine("Data " + art.IdArticulo);
        }
           
            DateTime fecha = DateTime.ParseExact(fecha, "dd-MM-yyy", System.Globalization.CultureInfo.InvariantCulture);

        Presupuesto presu = new Presupuesto();
        presu.Cliente = cl;
        presu.Articulos = aingresos;
        presu.fecha = 
        

         PresupuestoServices ps = new PresupuestoServices();
         ps.Crear(presu, npgsqlConnection);**/
    }

}
