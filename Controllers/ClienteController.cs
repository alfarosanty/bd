using Microsoft.AspNetCore.Mvc;

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
        List<Cliente> articulos = new ClienteServices().listarClientes(npgsqlConnection);
        con.cerrarConexion(npgsqlConnection);
        return articulos;
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


}
