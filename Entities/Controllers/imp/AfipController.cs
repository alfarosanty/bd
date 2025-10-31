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
    }
}
