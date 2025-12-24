using Microsoft.AspNetCore.Identity;

namespace CryptoAuth.DAL.Entities;

public class User : IdentityUser
{
    public string AvatarUrl { get; set; }
}