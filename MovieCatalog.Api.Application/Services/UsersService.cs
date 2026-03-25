using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Auth;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Config;
using MovieCatalog.Api.Common.Results;
using MovieCatalog.Api.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieCatalog.Api.Application.Services;

public class UsersService(UserManager<ApplicationUser> userManager,
    IOptions<JwtSettings> jwtOptions, IHttpContextAccessor httpContextAccessor) : IUsersService
{
    public async Task<Result<RegisteredDto>> RegisterAsync(RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            return Result<RegisteredDto>.BadRequest(errors);
        }

        await userManager.AddToRoleAsync(user, registerDto.Role);

        var registeredUser = new RegisteredDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = registerDto.Role
        };

        return Result<RegisteredDto>.Success(registeredUser);

    }

    public async Task<Result<string>> LoginAsync(LoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid Credentials."));
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid Credentials."));
        }

        //Issue JWT token
        var token = await GenerateToken(user);

        return Result<string>.Success(token);
    }

    public string UserId =>
       httpContextAccessor?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
       ?? httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
       ?? string.Empty;

    public bool IsInRole(string role)
    {
        return httpContextAccessor.HttpContext!.User.IsInRole(role);
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        // Implementation for JWT token generation goes here
        //setting up user claims
        var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
        };

        //Setting up user roles
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        claims = claims.Union(roleClaims).ToList();

        //Setting jwt key credentials 
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //Creating the token
        var token = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtOptions.Value.DurationInMinutes)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
