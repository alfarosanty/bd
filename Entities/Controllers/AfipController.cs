using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlumeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AfipController : ControllerBase
    {
    private readonly ILogger<ClienteController> _logger;

    public AfipController(ILogger<ClienteController> logger)
    {
        _logger = logger;
    }


        /// <summary>
        /// Endpoint de prueba para pedir un LoginTicket
        /// </summary>
        /// <returns>LoginTicketResponseData en JSON</returns>
        [HttpGet("login")]
        public async Task<IActionResult> ObtenerLoginTicket()
        {
            try
            {

                CConexion con = new CConexion();
                Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
                // Llamamos al servicio que ya obtiene los datos desde la BD
                AfipServices afipServices = new AfipServices();
                var loginTicket = await afipServices.AutenticacionAsync(verbose: true, npgsqlConnection);

                con.cerrarConexion(npgsqlConnection);


                return Ok(loginTicket); // Devuelve JSON con UniqueId, Sign, Token, etc.
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al generar el LoginTicket",
                    detalle = ex.Message
                });
            }
        }
    

[HttpPost("insertarCertificado")]
public IActionResult InsertarCertificado()
{
    try
    {
        // 1️⃣ Ruta del archivo
        string ruta = "C:\\Users\\Usuario\\Desktop\\Pruebas programa\\API_AFIP\\Documentos_AFIP_Autenticacion\\Produccion1\\BlumeProduccion1_687ab37dac3ccabb.pfx";

        // 2️⃣ Leer archivo como bytes
        byte[] certificadoBytes = System.IO.File.ReadAllBytes(ruta);

        // 3️⃣ Abrir conexión
        CConexion con = new CConexion();
        using var connection = con.establecerConexion();

        // 4️⃣ Update en PostgreSQL
        using var cmd = new Npgsql.NpgsqlCommand(
            "UPDATE \"DATOS_AFIP\" SET \"CERTIFICADO\" = @certificado",
            connection);

        cmd.Parameters.AddWithValue("@certificado", certificadoBytes);
        cmd.ExecuteNonQuery();

        con.cerrarConexion(connection);

        return Ok(new { mensaje = "Certificado insertado correctamente" });
    }
    catch (Exception ex)
    {
        return BadRequest(new
        {
            error = "Error al insertar certificado",
            detalle = ex.Message
        });
    }
}
    }
}
