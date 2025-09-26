using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class AfipServices
    {

        private readonly ILogger<AfipServices> _logger;

    public AfipServices(ILogger<AfipServices> logger)
    {
        _logger = logger;
    }

    public async Task<LoginTicketResponseData> AutenticacionAsync(string servicio, string urlWsaa, bool verbose = false)
    {
        try
        {
            LoginTicket loginTicket = new LoginTicket();
            LoginTicketResponseData loginTicketResponse = await loginTicket.ObtenerLoginTicketResponse(servicio, urlWsaa, verbose);
            return loginTicketResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo login ticket");
            throw;
        }
    }

}

