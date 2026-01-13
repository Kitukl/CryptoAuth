using Microsoft.AspNetCore.Http;

namespace CryptoAuth.BLL.DTOs;

public class UpdateUserRequest
{
    public string CurrentEmail { get; set; }
    public string? UserName { get; set; }
    public string? NewEmail { get; set; }
    public IFormFile? AvatarUrl { get; set; }
}