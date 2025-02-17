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
    public void Crear(int idTaller, string descripcion, string fecha,List<ArticuloCantidad> ri){
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

        //obtener taller
        //obtener fecha
        //obtener descripcion
        //listar articulos y cantidades
        //
        ArticuloServices ats = new ArticuloServices();
        List<ArticuloIngreso>   aingresos = new List<ArticuloIngreso> ();
        
        foreach(ArticuloCantidad art in ri){
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

        RemitoIngreso remitoIngreso = new RemitoIngreso();
      //  string date = DateTime.UtcNow.ToString("MM-dd-yyyy");
        //TallerServices ts = new TallerServices().Get(idTaller);          
        Taller taller  = new Taller();
        taller.Id = idTaller;
        remitoIngreso.Taller = taller;
        remitoIngreso.Fecha = DateTime.UtcNow;
        remitoIngreso.Articulos = aingresos;
        remitoIngreso.Descripcion = descripcion;
        new RemitoIngresoServices().crear(remitoIngreso,npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
    }

}
