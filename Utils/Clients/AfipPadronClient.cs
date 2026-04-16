using BlumeAPI.ServiceReference.PadronA13;
using BlumeAPI.ServiceReference.PadronA5;
using System.ServiceModel;
using System.Text.Json;

public class AfipPadronClient : IDisposable
{
    private readonly PersonaServiceA13 _clientA13;

    private readonly PersonaServiceA5 _clientA5;
    private readonly ILogger<AfipPadronClient> _logger;

    public AfipPadronClient(string endpointA13, string endpointA5, ILogger<AfipPadronClient> logger)
    {
        var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
        var addressA13 = new EndpointAddress(endpointA13);
        var addressA5 = new EndpointAddress(endpointA5);
        _clientA13 = new PersonaServiceA13Client(binding, addressA13);
        _clientA5 = new PersonaServiceA5Client(binding, addressA5);
        _logger = logger;
    }

    public async Task<string> ObtenerCategoriaFiscalAsync(string token, string sign, long cuitEmisor, long cuitConsultado)
    {
        var request = new getPersona_v2(token, sign, cuitEmisor, cuitConsultado);
        var response = await _clientA5.getPersona_v2Async(request);
        
        var retorno = response.personaReturn;

        // SERIALIZACIÓN: Convertimos el objeto a JSON para verlo en el log
        var opciones = new JsonSerializerOptions { WriteIndented = true };
        var jsonRetorno = JsonSerializer.Serialize(retorno, opciones);
        
        // Logueamos el JSON completo
        _logger.LogInformation("Respuesta completa de ARCA: {Json}", jsonRetorno);

        if (retorno.datosRegimenGeneral != null)
        {
            return "RESPONSABLE INSCRIPTO";
        }

        
        if (retorno.datosRegimenGeneral != null)
        {
            return "RESPONSABLE INSCRIPTO";
        }
        else if (retorno.datosMonotributo != null)
        {
            return "MONOTRIBUTO";
        }
        
        return "Otro / Exento";
    }

    public async Task<ArcaPersonaDto> ObtenerPersonaAsync(string token, string sign, long cuitEmisor, long cuitConsultado)
    {
        try
        {
            var request = new BlumeAPI.ServiceReference.PadronA13.getPersona(token, sign, cuitEmisor, cuitConsultado);
            var response = await _clientA13.getPersonaAsync(request);
            _logger.LogDebug(message: "Respuesta recibida: {@Response}", response);
            var data = response.personaReturn;
            if (data?.persona == null)
        {
            _logger.LogWarning("AFIP no devolvió datos para el CUIT {Cuit}", cuitConsultado);
            throw new Exception("CUIT no encontrado en AFIP.");
        }
            
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
                Contacto = persona.nombre + " " + persona.apellido,
                Domicilio = (domicilio?.direccion ?? "S/D").ToUpper(),
                Localidad = (domicilio?.localidad ?? "S/L").ToUpper(),
                Provincia =  (domicilio?.descripcionProvincia ?? "S/P").ToUpper(),
                tipoPersona = (persona.tipoClave ?? "SIN DETERMINAR").ToUpper(), 
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
    if (_clientA13 is IDisposable disposableA13)
    {
        disposableA13.Dispose();
    }

    if (_clientA5 is IDisposable disposableA5)
    {
        disposableA5.Dispose();
    }
}
}