namespace CryptoAuth.DAL.Entities;

public class RefreshToken
{
    public string Token { get; set; }
    public string UserEmail { get; set; }
    public DateTime Expire { get; set; }
}