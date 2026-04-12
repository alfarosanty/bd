namespace BlumeAPI.Services;
public interface IARCAService
{
    Task<LoginTicketResponseData> AutenticacionAsync(string servicio);
    Task<ArcaPersonaDto?> ConsultarPersonaAsync(long cuit);

}