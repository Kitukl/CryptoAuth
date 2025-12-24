namespace CryptoAuth.BLL.DTOs;

public class UpdateUserRequest
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
}