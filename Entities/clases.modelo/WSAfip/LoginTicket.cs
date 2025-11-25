
using System.Text;
using System.Xml;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;



public class LoginTicket
{ 
    public UInt32 UniqueId; // Entero de 32 bits sin signo que identifica el requerimiento
    public DateTime GenerationTime; // Momento en que fue generado el requerimiento
    public DateTime ExpirationTime; // Momento en el que expira la solicitud
    public string? Service; // Identificacion del WSN para el cual se solicita el TA
    public string? Sign; // Firma de seguridad recibida en la respuesta
    public string? Token; // Token de seguridad recibido en la respuesta
    public XmlDocument? XmlLoginTicketRequest = null;
    public XmlDocument? XmlLoginTicketResponse = null;

    public string XmlStrLoginTicketRequestTemplate = "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";
    private bool _verboseMode = true;
    private static UInt32 _globalUniqueID = 0; // OJO! NO ES THREAD-SAFE

    /// <summary>
    /// Construye un Login Ticket obtenido del WSAA
    /// </summary>
    /// <param name="argServicio">Servicio al que se desea acceder</param>
    /// <param name="argUrlWsaa">URL del WSAA</param>
    /// <param name="argRutaCertX509Firmante">Ruta del certificado X509 (con clave privada) usado para firmar</param>
    /// <param name="argPassword">Password del certificado X509 (con clave privada) usado para firmar</param>
    /// <param name="argProxy">IP:port del proxy</param>
    /// <param name="argProxyUser">Usuario del proxy</param>''' 
    /// <param name="argProxyPassword">Password del proxy</param>
    /// <param name="argVerbose">Nivel detallado de descripcion? true/false</param>
    /// <remarks></remarks>
    public async Task<LoginTicketResponseData> ObtenerLoginTicketResponse(byte[] argcertBytes, SecureString argpasswordSecure , string argServicio, string argUrlWsaa, bool argVerbose)
    {
        const string ID_FNC = "[ObtenerLoginTicketResponse]";
        this._verboseMode = argVerbose;
        CertificadosX509Lib.VerboseMode = argVerbose;
        string? cmsFirmadoBase64 = null;
        string? loginTicketResponse = null;
        XmlNode? xmlNodoUniqueId = null;
        XmlNode? xmlNodoGenerationTime = null;
        XmlNode? xmlNodoExpirationTime = null;
        XmlNode? xmlNodoService = null;

        // PASO 1: Genero el Login Ticket Request
        try
        {
            _globalUniqueID += 1;

            XmlLoginTicketRequest = new XmlDocument();
            XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

            xmlNodoUniqueId      = XmlLoginTicketRequest.SelectSingleNode("//uniqueId") 
                                    ?? throw new Exception("Falta nodo <uniqueId>");
            xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime") 
                                    ?? throw new Exception("Falta nodo <generationTime>");
            xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime") 
                                    ?? throw new Exception("Falta nodo <expirationTime>");
            xmlNodoService   = XmlLoginTicketRequest.SelectSingleNode("//service") 
                                    ?? throw new Exception("Falta nodo <service>");

            xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s");
            xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+10).ToString("s");
            xmlNodoUniqueId.InnerText = _globalUniqueID.ToString();
            xmlNodoService.InnerText = argServicio;

            this.Service = argServicio;

            if (this._verboseMode) 
                Console.WriteLine(XmlLoginTicketRequest.OuterXml);
        }
        catch (Exception ex) 
        {
            throw new Exception(ID_FNC + "***Error GENERANDO el LoginTicketRequest : " + ex.Message, ex);
        }


        // PASO 2: Firmo el Login Ticket Request
        try
        {
            if (this._verboseMode) Console.WriteLine(ID_FNC + "***Leyendo certificado desde BD");

            // Abrimos la conexión usando tu clase CConexion
            
            // Cargamos el certificado en X509Certificate2
            X509Certificate2 certFirmante = CertificadosX509Lib.ObtieneCertificadoDesdeBytes(argcertBytes, argpasswordSecure);

            if (this._verboseMode)
            {
                Console.WriteLine(ID_FNC + "***Firmando LoginTicketRequest:");
                Console.WriteLine(XmlLoginTicketRequest.OuterXml);
            }

            // Convertimos el Login Ticket Request a bytes, lo firmamos y lo convertimos a Base64
            byte[] msgBytes = Encoding.UTF8.GetBytes(XmlLoginTicketRequest.OuterXml);
            byte[] encodedSignedCms = CertificadosX509Lib.FirmaBytesMensaje(msgBytes, certFirmante);
            cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);
        }
        catch (Exception excepcionAlFirmar)
        {
            throw new Exception(ID_FNC + "***Error FIRMANDO el LoginTicketRequest : " + excepcionAlFirmar.Message);
        }

        // PASO 3: Invoco al WSAA para obtener el Login Ticket Response
        try
        {
            if (this._verboseMode)
            {
                Console.WriteLine(ID_FNC + "***Llamando al WSAA en URL: {0}", argUrlWsaa);
                Console.WriteLine(ID_FNC + "***Argumento en el request:");
                Console.WriteLine(cmsFirmadoBase64);
            }
    
            var servicioWsaa = new AFIP.Wsaa.LoginCMSClient();
            servicioWsaa.Endpoint.Address = new System.ServiceModel.EndpointAddress(argUrlWsaa);
            var response = await servicioWsaa.loginCmsAsync(cmsFirmadoBase64);
            Console.WriteLine(response);
            loginTicketResponse = response.loginCmsReturn;


            if (this._verboseMode)
            {
                Console.WriteLine(ID_FNC + "***LoguinTicketResponse: ");
                Console.WriteLine(loginTicketResponse);
            }

        }
        catch (Exception excepcionAlInvocarWsaa)
        {
            throw new Exception(ID_FNC + "***Error INVOCANDO al servicio WSAA : " + excepcionAlInvocarWsaa.Message);
        }

        // PASO 4: Analizo el Login Ticket Response recibido del WSAA
        try
        {
            XmlLoginTicketResponse = new XmlDocument();
            XmlLoginTicketResponse.LoadXml(loginTicketResponse);

            // Creamos objeto para devolver
            var loginTicketData = new LoginTicketResponseData();

            XmlNode nodo;

            nodo = XmlLoginTicketResponse.SelectSingleNode("//uniqueId") 
                ?? throw new Exception("No se encontró el nodo <uniqueId> en la respuesta del WSAA");
            loginTicketData.UniqueId = UInt32.Parse(nodo.InnerText);

            nodo = XmlLoginTicketResponse.SelectSingleNode("//generationTime") 
                ?? throw new Exception("No se encontró el nodo <generationTime> en la respuesta del WSAA");
            loginTicketData.GenerationTime = DateTime.Parse(nodo.InnerText);

            nodo = XmlLoginTicketResponse.SelectSingleNode("//expirationTime") 
                ?? throw new Exception("No se encontró el nodo <expirationTime> en la respuesta del WSAA");
            loginTicketData.ExpirationTime = DateTime.Parse(nodo.InnerText);

            nodo = XmlLoginTicketResponse.SelectSingleNode("//sign") 
                ?? throw new Exception("No se encontró el nodo <sign> en la respuesta del WSAA");
            loginTicketData.Sign = nodo.InnerText;

            nodo = XmlLoginTicketResponse.SelectSingleNode("//token") 
                ?? throw new Exception("No se encontró el nodo <token> en la respuesta del WSAA");
            loginTicketData.Token = nodo.InnerText;

            // Devolvemos como JSON
            return loginTicketData;
        }
        catch (Exception excepcionAlAnalizarLoginTicketResponse)
        {
            throw new Exception(ID_FNC + "***Error ANALIZANDO el LoginTicketResponse : " + excepcionAlAnalizarLoginTicketResponse.Message, excepcionAlAnalizarLoginTicketResponse);
        }

    }
}



