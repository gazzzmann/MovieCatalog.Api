using MovieCatalog.Api.Application.DTOs.Favorite;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;

namespace MovieCatalog.Api.Application.Contracts
{
    public interface IFavoriteService
    {
        Task<Result> AddFavoriteAsync(int movieId);
        Task<Result<PagedResult<FavoriteMovieDto>>> GetFavoritesAsync(PaginationParameters paginationParameters);
        Task<Result> RemoveFavoriteAsync(int movieId);
    }
}