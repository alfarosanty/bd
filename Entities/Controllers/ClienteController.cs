using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Npgsql;


namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{

    private readonly IClienteService _clienteService;
    private readonly IARCAService _arcaService;

    public ClienteController(IClienteService clienteService, IARCAService arcaService)
    {
        _clienteService = clienteService;
        _arcaService = arcaService;
    }
/*=================== NUEVOS MÉTODOS BIEN HECHOS ===================*/
[HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int? page, 
        [FromQuery] int? pageSize, 
        [FromQuery] FiltrosClienteDTO filtros)
    {
        var resultado = await _clienteService.GetClientesAsync(page, pageSize, filtros);
        return Ok(resultado);
    }

[HttpGet("{idCliente}")]
    public async Task<IActionResult> GetById(int idCliente)
    {
        var cliente = await _clienteService.GetById(idCliente);
        if (cliente == null) return NotFound(); // Manejo prolijo
        return Ok(cliente);
    }

[HttpGet("condiciones-fiscales")]
    public async Task<IActionResult> GetCondicionFiscal()
    {
        var condicionesFiscales = await _clienteService.GetCondicionFiscalsAsync();   
        return Ok(condicionesFiscales);
    }

[HttpPost]
    public async Task<IActionResult> Crear([FromBody] Cliente cliente)
    {
        try
        {
            var id = await _clienteService.Guardar(cliente); 
            return Ok(new { id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

[HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] Cliente cliente)
    {
        if (id != cliente.Id) return BadRequest("El ID del cliente no coincide");
        
        try
        {
            await _clienteService.Guardar(cliente);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

[HttpGet("consultar-afip/{cuit}")]
    public async Task<IActionResult> ConsultarAFIP(long cuit)
    {
        try
        {
            var datosCliente = await _arcaService.ConsultarPersonaAsync(cuit);
            
            if (datosCliente == null) return NotFound("CUIT no encontrado");
            
            return Ok(datosCliente);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error consultando ARCA: {ex.Message}");
        }
    }


}
