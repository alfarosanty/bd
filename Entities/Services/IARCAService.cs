namespace BlumeAPI.Services;
public interface IARCAService
{
    Task<LoginTicketResponseData> AutenticacionAsync();

}