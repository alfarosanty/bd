using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

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
[HttpGet("login/facturaElectronica")]
    public async Task<IActionResult> ObtenerLoginTicketWSFE()
    {
        try
        {

            var loginTicket = await _arcaService.AutenticacionAsync("wsfe");

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

[HttpGet("login/padron")]
    public async Task<IActionResult> ObtenerLoginTicketPadron()
    {
        try
        {

            var loginTicket = await _arcaService.AutenticacionAsync("ws_sr_padron_a13");

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
    public async Task<IActionResult> InsertarCertificado(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Archivo no válido.");

        try
        {
            using var memoryStream = new MemoryStream();
            await archivo.CopyToAsync(memoryStream);
            
            await _arcaService.GuardarCertificadoAsync(memoryStream.ToArray());

            return Ok(new { mensaje = "Certificado guardado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

