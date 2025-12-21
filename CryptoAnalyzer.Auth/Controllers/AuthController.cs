using CryptoAuth.BLL.Commands;
using CryptoAuth.BLL.Commands.LoginCommandHandler;
using CryptoAuth.BLL.Commands.RefreshTokenCommandHandler;
using CryptoAuth.BLL.Commands.RegisterCommandHandler;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptoAnalyzer.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _mediatr;
    private readonly IHttpContextAccessor _accessor;

    public AuthController(ISender mediatr, IHttpContextAccessor accessor)
    {
        _mediatr = mediatr;
        _accessor = accessor;
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterCommand command)
    {
        var result = await _mediatr.Send(command);

        if (!result.isSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginCommand command)
    {
        var result = await _mediatr.Send(command);

        if (!result.isSuccess)
        {
            return BadRequest(result.Errors);
        }
        var token = result.Value;

        var context = _accessor.HttpContext;

        context?.Response.Cookies.Append("jwt", token.AccesToken);

        return Ok(token);
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<string>> Refresh(RefreshTokenCommand command)
    {
        var result = await _mediatr.Send(command);
        if (!result.isSuccess)
        {
            return Unauthorized(result.Errors);
        }
        var token = result.Value;
        var context = _accessor.HttpContext;

        context?.Response.Cookies.Delete("jwt");
        context?.Response.Cookies.Append("jwt", token.AccesToken);
        
        return Ok(token);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<string>> ForgotPass(ForgotPasswordCommand command)
    {
        var result = await _mediatr.Send(command);
        if (!result.isSuccess) return BadRequest(result.Errors);
        return Ok(result.Value);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<string>> ResetPassword(ResetPasswordCommand command)
    {
        var result = await _mediatr.Send(command);
        if (!result.isSuccess) return BadRequest(result.Errors);
        return result.Value;
    }

    [HttpPost("logout")]
    public async Task<ActionResult<string>> Logout(LogoutCommand command)
    {
        var result = await _mediatr.Send(command);
        if (!result.isSuccess)
        {
            return BadRequest(result.Errors);
        }
        var context = _accessor.HttpContext;
        context?.Response.Cookies.Delete("jwt");

        return Ok("User logout!");
    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult<string>> ConfirmEmail(ConfirmEmailCommand command)
    {
        var result = await _mediatr.Send(command);
        if (!result.isSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }
}