public class LoginTicketResponseData
{
    public string? Token { get; set; }
    public string? Sign { get; set; }
    public DateTime GenerationTime { get; set; }
    public DateTime ExpirationTime { get; set; }
    public uint UniqueId { get; set; }
}
