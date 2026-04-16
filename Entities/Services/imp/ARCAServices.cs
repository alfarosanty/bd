using System.Security;
using BlumeAPI.Entities;
using BlumeAPI.Services;
using Microsoft.Extensions.Options;
using Serilog;

public class ARCAService : IARCAService
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly AfipSettings _afipSettings;
    private readonly AfipPadronClient _padronClient;

    public ARCAService( IUnitOfWork iUnitOfWork,
                        IOptions<AfipSettings> afipSettings,
                        AfipPadronClient padronClient
    )
    {
        _unitOfWork = iUnitOfWork;
        _afipSettings = afipSettings.Value;
        _padronClient = padronClient;
    }

    public async Task<LoginTicketResponseData> AutenticacionAsync(string servicio)    
    {
        Console.WriteLine("Iniciando proceso de autenticación...", servicio);

        try
        {
            // 1️⃣ Buscamos el token actual usando tu nuevo repositorio
            var auth = await _unitOfWork.Arca.ObtenerAutenticacionPorServicioAsync(servicio);

            if (auth != null && auth.Expiracion.ToUniversalTime() > DateTime.UtcNow)
            {
                Console.WriteLine("Token válido encontrado en base de datos.");
                return new LoginTicketResponseData
                {
                    Token = auth.Token,
                    Sign = auth.Firma,
                    ExpirationTime = auth.Expiracion,
                    GenerationTime = DateTime.Now,
                    UniqueId = auth.UniqueId,
                    Service = servicio
                };
            }

            Console.WriteLine("Token expirado o inexistente. Solicitando nuevo...");

            // 2️⃣ Obtenemos configuración desde la BD
            var config = await _unitOfWork.Arca.ObtenerConfiguracionAsync() 
                 ?? throw new Exception($"No se encontró configuración para el servicio {servicio}");

            // 3️⃣ Construimos SecureString
            SecureString passwordSecure = new SecureString();
            foreach (char c in config.Contrasena) passwordSecure.AppendChar(c);
            passwordSecure.MakeReadOnly();

            // 4️⃣ Solicitamos nuevo LoginTicket a ARCA
            var loginTicket = new LoginTicket();
            var loginResponse = await loginTicket.ObtenerLoginTicketResponse(
                config.Certificado,
                passwordSecure,
                servicio,
                config.UrlWsaa,
                true // verbose
            );

            Console.WriteLine("LoginTicket generado exitosamente.");

            // 5️⃣ Guardamos el nuevo token en la BD usando el Repositorio
            var nuevaAuth = new DatosAutenticacion
            {
                UniqueId = loginResponse.UniqueId,
                Token = loginResponse.Token,
                Firma = loginResponse.Sign,
                Expiracion = loginResponse.ExpirationTime,
                Servicio = servicio
            };

            await _unitOfWork.Arca.GuardarAutenticacionAsync(nuevaAuth);
            await _unitOfWork.SaveChangesAsync(); // Persistimos los cambios

            loginResponse.Service = servicio;

            return loginResponse;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR crítico en autenticación: {ex.Message}");
            throw;
        }
    }



    public async Task<Cliente?> ConsultarPersonaAsync(long cuit)
    {
        try
        {
            string servicioPadronA13 = "ws_sr_padron_a13";
            string servicioPadronA5 = "ws_sr_constancia_inscripcion";

            var ticketPadronA13 = await AutenticacionAsync(servicioPadronA13);

            var personaARCADTO = await _padronClient.ObtenerPersonaAsync(
                ticketPadronA13.Token, 
                ticketPadronA13.Sign, 
                long.Parse(_afipSettings.Cuit), 
                cuit
            );

            var ticketPadronA5 = await AutenticacionAsync(servicioPadronA5);
            var condicionFiscal = await _padronClient.ObtenerCategoriaFiscalAsync(
                ticketPadronA5.Token,
                ticketPadronA5.Sign,
                long.Parse(_afipSettings.Cuit),
                cuit
                );

            var condicionesFiscales = await _unitOfWork.Clientes.GetCondicionFiscalsAsync();

            personaARCADTO.CondicionFiscal = condicionesFiscales.Find(condicion => condicion.Descripcion == condicionFiscal);

            var cliente = new Cliente
            {
                RazonSocial = personaARCADTO.RazonSocial,
                Contacto = personaARCADTO.Contacto,
                Cuit = cuit.ToString(),
                Telefono = personaARCADTO.Telefono,
                Domicilio = personaARCADTO.Domicilio,
                Localidad = personaARCADTO.Localidad,
                Provincia = personaARCADTO.Provincia,
                CondicionFiscal = personaARCADTO.CondicionFiscal,
                Valido = personaARCADTO.EsValido
            };



            return cliente;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error al consultar AFIP para CUIT {Cuit}", cuit);
            return null;
        }
    }

    public async Task GuardarCertificadoAsync(byte[] certificadoBytes)
    {
        var datosAfip = await _unitOfWork.Arca.GetPrimerRegistroAsync();

        if (datosAfip == null)
            throw new Exception("No existe el registro de configuración AFIP en la base de datos.");

        datosAfip.Certificado = certificadoBytes;
        
        _unitOfWork.Arca.Update(datosAfip);
        await _unitOfWork.SaveChangesAsync();
    }

}

