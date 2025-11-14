using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly IClienteService _clienteService;

        public ClienteController(ILogger<ClienteController> logger, IClienteService clienteService)
        {
            _logger = logger;
            _clienteService = clienteService;
        }

        [HttpGet("GetClientes")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var clientes = await _clienteService.ListarClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("GetClienteById/{idCliente}")]
        public async Task<ActionResult<Cliente>> GetClienteById(int idCliente)
        {
            var cliente = await _clienteService.GetClienteAsync(idCliente);

            if (cliente == null)
                return NotFound($"No se encontró el cliente con ID {idCliente}");

            return Ok(cliente);
        }

        [HttpGet("GetCondicionFiscal")]
        public async Task<ActionResult<IEnumerable<CondicionFiscal>>> GetCondicionFiscal()
        {
            var condiciones = await _clienteService.GetCondicionesFiscalesAsync();
            return Ok(condiciones);
        }

        [HttpPost("Crear")]
        public async Task<ActionResult<Cliente>> Crear([FromBody] Cliente cliente)
        {
            try
            {
                var nuevoCliente = await _clienteService.CrearAsync(cliente);
                return CreatedAtAction(nameof(GetClienteById), new { idCliente = nuevoCliente.Id }, nuevoCliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente");
                return StatusCode(500, $"Error al crear el cliente: {ex.Message}");
            }
        }

        [HttpPut("Actualizar")]
        public async Task<ActionResult> Actualizar([FromBody] Cliente cliente)
        {
            try
            {
                var filasAfectadas = await _clienteService.ActualizarAsync(cliente);
                if (filasAfectadas == 0)
                    return NotFound($"No se encontró el cliente con ID {cliente.Id}");

                return Ok($"Cliente {cliente.Id} actualizado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el cliente");
                return StatusCode(500, $"Error al actualizar el cliente: {ex.Message}");
            }
        }
    }
}
