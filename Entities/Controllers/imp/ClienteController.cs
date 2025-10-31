using Microsoft.AspNetCore.Mvc;
using Npgsql;


namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ClienteController : ControllerBase
{

    private readonly ILogger<ClienteController> _logger;

    public ClienteController(ILogger<ClienteController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetClientes")]
    public IEnumerable<Cliente> Get()
    {
        
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        List<Cliente> clientes = new ClienteServices().listarClientes(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return clientes;
    }

     [HttpGet("GetClienteById/{idCliente}")]
    public Cliente GetById(int idCliente)
    {
        CConexion con =  new CConexion();
        Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();
        Cliente cliente = new ClienteServices().GetCliente(idCliente, npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return cliente;
    }

[HttpGet("GetCondicionFiscal")]
public IEnumerable<CondicionFiscal> GetCondicionFiscal()
{
    CConexion con = new CConexion();
    Npgsql.NpgsqlConnection npgsqlConnection = con.establecerConexion();

    ClienteServices clienteServices = new ClienteServices();
    List<CondicionFiscal> condicionesFiscales = clienteServices.GetCondicionFiscal(npgsqlConnection);

    con.cerrarConexion(npgsqlConnection);
    return condicionesFiscales;
}

[HttpPost("Crear")]
public IActionResult Crear([FromBody] Cliente cliente)
{
    CConexion con = new CConexion();
    NpgsqlConnection npgsqlConnection = con.establecerConexion();

    try
    {
        var clienteServices = new ClienteServices();
        var clienteCreado = clienteServices.Crear(npgsqlConnection, cliente);
        return Ok(clienteCreado);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return StatusCode(500, $"Error al crear el cliente: {ex.Message}");
    }
    finally
    {
        con.cerrarConexion(npgsqlConnection); // cerrar manualmente
    }
}


[HttpPost("Actualizar")]
public IActionResult Actualizar([FromBody] Cliente cliente)
{
    CConexion con = new CConexion();
    NpgsqlConnection npgsqlConnection = con.establecerConexion();

    try
    {
        var clienteServices = new ClienteServices();
        var clienteActualizado = clienteServices.Actualizar(npgsqlConnection, cliente);
        return Ok(clienteActualizado);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return StatusCode(500, $"Error al actualizar el cliente: {ex.Message}");
    }
    finally
    {
        con.cerrarConexion(npgsqlConnection); // cerrar manualmente
    }
}


}
