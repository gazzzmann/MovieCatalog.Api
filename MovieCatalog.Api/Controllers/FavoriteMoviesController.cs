using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Favorite;
using MovieCatalog.Api.Common.Models.Paging;

namespace MovieCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoriteMoviesController(IFavoriteService favoriteService) : BaseApiController
    {
        // GET: api/FavoriteMovies
        [HttpGet]
        public async Task<ActionResult<PagedResult<FavoriteMovieDto>>> GetFavorites
         ([FromQuery] PaginationParameters paginationParameters)
        {
            var result = await favoriteService.GetFavoritesAsync(paginationParameters);
            return ToActionResult(result);
        }

        // POST: api/FavoriteMovies
        [HttpPost]
        public async Task<IActionResult> AddFavorite(AddFavoriteDto dto)
        {
            var result = await favoriteService.AddFavoriteAsync(dto.MovieId);
            return ToActionResult(result);
        }

        // DELETE: api/FavoriteMovies/5
        [HttpDelete("{movieId}")]
        public async Task<IActionResult> RemoveFavorite(int movieId)
        {
            var result = await favoriteService.RemoveFavoriteAsync(movieId);
            return ToActionResult(result);
        }
    }
}