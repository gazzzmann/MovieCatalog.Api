using MovieCatalog.Api.Application.DTOs.Auth;
using MovieCatalog.Api.Common.Results;

namespace MovieCatalog.Api.Application.Contracts
{
    public interface IUsersService
    {
        string UserId { get; }

        bool IsInRole(string role);
        Task<Result<string>> LoginAsync(LoginDto dto);
        Task<Result<RegisteredDto>> RegisterAsync(RegisterDto registerDto);
    }
}