namespace BlumeAPI.Services{

public interface IPdfService
{
    Task<byte[]> convertHtmlToPdfAsync(string html);
}

}
