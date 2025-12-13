namespace CryptoAuth.BLL.DTOs;

public class RessetPasswordRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}