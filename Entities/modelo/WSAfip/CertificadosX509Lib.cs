
using System.Security;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;



public class CertificadosX509Lib
{
    public static bool VerboseMode = false;

    /// <summary>
    /// Firma mensaje
    /// </summary>
    /// <param name="argBytesMsg">Bytes del mensaje</param>
    /// <param name="argCertFirmante">Certificado usado para firmar</param>
    /// <returns>Bytes del mensaje firmado</returns>
    public static byte[] FirmaBytesMensaje(byte[] argBytesMsg, X509Certificate2 argCertFirmante)
    {
        const string ID_FNC = "[FirmaBytesMensaje]";
        try
        {
            ContentInfo infoContenido = new ContentInfo(argBytesMsg);
            SignedCms cmsFirmado = new SignedCms(infoContenido);

            CmsSigner cmsFirmante = new CmsSigner(argCertFirmante)
            {
                IncludeOption = X509IncludeOption.EndCertOnly
            };

            if (VerboseMode) Console.WriteLine(ID_FNC + "***Firmando bytes del mensaje...");

            cmsFirmado.ComputeSignature(cmsFirmante);

            if (VerboseMode) Console.WriteLine(ID_FNC + "***OK mensaje firmado");

            return cmsFirmado.Encode();
        }
        catch (Exception excepcionAlFirmar)
        {
            throw new Exception(ID_FNC + "***Error al firmar: " + excepcionAlFirmar.Message);
        }
    }

    /// <summary>
    /// Obtiene certificado desde un arreglo de bytes (por ejemplo desde la base de datos)
    /// </summary>
    /// <param name="certBytes">Bytes del certificado .pfx</param>
    /// <param name="argPassword">Contraseña del certificado (puede ser null si no tiene)</param>
    /// <returns>Un objeto X509Certificate2 listo para usar</returns>
public static X509Certificate2 ObtieneCertificadoDesdeBytes(byte[] certBytes, SecureString argPassword)
{
    const string ID_FNC = "[ObtieneCertificadoDesdeBytes]";
    try
    {
        X509Certificate2 objCert;

        if (argPassword != null && argPassword.Length > 0)
        {
            // Constructor que acepta byte[] + SecureString + X509KeyStorageFlags
            objCert = new X509Certificate2(certBytes, argPassword, X509KeyStorageFlags.PersistKeySet);
        }
        else
        {
            // Constructor que acepta solo byte[]
            objCert = new X509Certificate2(certBytes);
        }

        if (VerboseMode) Console.WriteLine(ID_FNC + "***Certificado cargado desde bytes OK");

        return objCert;
    }
    catch (Exception ex)
    {
        throw new Exception(ID_FNC + "***Error al cargar certificado desde bytes: " + ex.Message);
    }
}

}