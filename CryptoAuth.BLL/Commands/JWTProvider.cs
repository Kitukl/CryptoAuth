using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using CryptoAuth.BLL.DTOs;
using CryptoAuth.DAL.Entities;
using CryptoAuth.DAL.Repositories;
using Microsoft.Extensions.Configuration;

namespace CryptoAuth.BLL.Commands;

public class JWTProvider
{
    private readonly IRepository<RefreshToken> _repository;
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly JWTOptions _jwtOptions;

    public JWTProvider(IOptions<JWTOptions> jwtOptions, IRepository<RefreshToken> repository, IConfiguration configuration, UserManager<User> userManager)
    {
        _repository = repository;
        _configuration = configuration;
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(User user)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha512);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.Now.AddHours(_jwtOptions.Expires)
            );
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenValue;
    }

    public async Task<Result<LoginResponse>> ValidateRefreshToken(string token)
    {
        var refreshToken = await _repository.GetByIdAsync(token);

        if (string.IsNullOrEmpty(refreshToken.Token)) return Result<LoginResponse>.Failure("Refresh token not found");
        if (refreshToken.Expire < DateTime.UtcNow) return Result<LoginResponse>.Failure("Refresh token expired");

        await _repository.DeleteAsync(token);

        var user = await _userManager.FindByEmailAsync(refreshToken.UserEmail);
        return Result<LoginResponse>.Success(new LoginResponse()
        {
            AccesToken = GenerateToken(user),
            RefreshToken = await GenerateRefreshToken(user)
        });
    }

    public async Task<string> GenerateRefreshToken(User user)
    {
        var expire = _configuration.GetValue<int>("JwtOptions:RefreshToken");
        var token = new RefreshToken()
        {
            Token = Convert.ToString(Guid.NewGuid()) ?? String.Empty,
            UserEmail = user.Email,
            Expire = DateTime.UtcNow.AddHours(expire)
        };

        var tokenValue = await _repository.CreateAsync(token);
        return tokenValue;
    }
}

public class JWTOptions
{
    public string SecretKey { get; set; }
    public int Expires { get; set; }
}