using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlumeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ARCAController : ControllerBase
    {
    private readonly IARCAService _arcaService;

    public ARCAController(IARCAService arcaService)
    {
        _arcaService = arcaService;
    
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

                var loginTicket = await _arcaService.AutenticacionAsync();

                return Ok(loginTicket);
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
        string ruta = "C:\\Fran\\ARCA\\CERTIFICADOS\\blumeproduccion2wsfe\\facturador1wsfe.pfx";

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
