namespace CryptoAuth.DAL.Entities;

public class ResetPasswordCode
{
    public string Code { get; set; }
    public string UserEmail { get; set; }
    public DateTime Expire { get; set; }
}