namespace CryptoAuth.BLL.DTOs;

public class ConfirmEmailRequest
{
    public string userID { get; set; }
    public string token { get; set; }
}