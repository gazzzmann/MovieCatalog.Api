using MovieCatalog.Api.Application.DTOs.Genre;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;

namespace MovieCatalog.Api.Application.Contracts
{
    public interface IGenresService
    {
        Task<Result<GetGenreDto>> CreateGenreAsync(CreateGenreDto genreDto);
        Task<Result> DeleteGenreAsync(int id);
        Task<bool> GenreExistsAsync(int id);
        Task<bool> GenreExistsAsync(string name);
        Task<Result<GetGenreDto>> GetGenreByIdAsync(int id);
        Task<Result<PagedResult<GetGenresDto>>> GetGenresAsync(PaginationParameters paginationParameters);
        Task<Result> UpdateGenreAsync(int id, UpdateGenreDto genreDto);
    }
}