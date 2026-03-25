using MovieCatalog.Api.Application.DTOs.Movie;
using MovieCatalog.Api.Common.Models.Filters;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;

namespace MovieCatalog.Api.Application.Contracts
{
    public interface IMoviesService
    {
        Task<Result<GetMoviesDto>> CreateMovieAsync(CreateMovieDto movieDto);
        Task<Result> DeleteMovieAsync(int id);
        Task<Result<GetMoviesDto>> GetMovieByIdAsync(int id);
        Task<Result<PagedResult<GetMoviesSlimDto>>> GetMoviesAsync
            (PaginationParameters paginationParameters, MovieFilterParameters movieFilters);
        Task<Result<PagedResult<GetMoviesDto>>> GetMyMoviesAsync(PaginationParameters paginationParameters);
        Task<bool> MovieExistsAsync(int id);
        Task<Result> UpdateMovieAsync(int id, UpdateMovieDto movieDto);
    }
}