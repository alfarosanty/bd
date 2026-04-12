using BlumeAPI.ServiceReference.Padron;
using System.ServiceModel;

public class AfipPadronClient : IDisposable
{
    private readonly PersonaServiceA13 _client;
    private readonly ILogger<AfipPadronClient> _logger;

    public AfipPadronClient(string endpoint, ILogger<AfipPadronClient> logger)
    {
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        var address = new EndpointAddress(endpoint);
        _client = new PersonaServiceA13Client(binding, address);
        _logger = logger;
    }

    public async Task<ArcaPersonaDto> ObtenerPersonaAsync(string token, string sign, long cuitEmisor, long cuitConsultado)
    {
        try
        {
            var request = new getPersona(token, sign, cuitEmisor, cuitConsultado);
            var response = await _client.getPersonaAsync(request);
            _logger.LogDebug(message: "Respuesta recibida: {@Response}", response);
            var data = response.personaReturn;
            if (data?.persona == null)
        {
            _logger.LogWarning("AFIP no devolvió datos para el CUIT {Cuit}", cuitConsultado);
            throw new Exception("CUIT no encontrado en AFIP.");
        }
            
            // Validación de errores de AFIP
            if (data?.persona == null)
                {
                    throw new Exception("CUIT no encontrado o sin datos registrados en AFIP.");
                }

            var persona = data.persona;
            string razonSocial = !string.IsNullOrWhiteSpace(persona.razonSocial) 
                                    ? persona.razonSocial 
                                    : $"{persona.nombre ?? ""} {persona.apellido ?? ""}".Trim();
            var domicilio = persona.domicilio?.FirstOrDefault();
                
           return new ArcaPersonaDto
            {
                Cuit = cuitConsultado.ToString(),
                RazonSocial = razonSocial.ToUpper(),
                Domicilio = (domicilio?.direccion ?? "S/D").ToUpper(),
                Localidad = (domicilio?.localidad ?? "S/L").ToUpper(),
                Provincia = AfipHelper.ObtenerNombreProvincia(domicilio?.idProvincia ?? 0),
                CondicionIva = (persona.tipoClave ?? "SIN DETERMINAR").ToUpper(), 
                EsValido = true
            };
        }
        catch (CommunicationException ex)
        {
            // Error de conexión con AFIP
            throw new Exception("Error de red al conectar con los servidores de AFIP.", ex);
        }
    }

    public void Dispose()
    {
        if (_client is IDisposable disposableClient)
        {
            disposableClient.Dispose();
        }
    }
}