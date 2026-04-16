namespace BlumeAPI.Services;
public interface IARCAService
{
    Task<LoginTicketResponseData> AutenticacionAsync(string servicio);
    Task<Cliente?> ConsultarPersonaAsync(long cuit);
    Task GuardarCertificadoAsync(byte[] certificadoBytes);

}