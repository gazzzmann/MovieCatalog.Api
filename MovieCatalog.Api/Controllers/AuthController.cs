using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Auth;

namespace MovieCatalog.Api.Controllers;
// Comtroller

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IUsersService usersService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisteredDto>> Register(RegisterDto registerDto)
    {
        var result = await usersService.RegisterAsync(registerDto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto loginDto)
    {
        var result = await usersService.LoginAsync(loginDto);
        return ToActionResult(result);
    }
}