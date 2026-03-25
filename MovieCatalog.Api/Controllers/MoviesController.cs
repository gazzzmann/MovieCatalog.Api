using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Movie;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Filters;
using MovieCatalog.Api.Common.Models.Paging;

namespace MovieCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController(IMoviesService moviesService) : BaseApiController
    {

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<PagedResult<GetMoviesSlimDto>>> GetMovies
            ([FromQuery] PaginationParameters paginationParameters,
            [FromQuery] MovieFilterParameters movieFilters)
        {
            var result = await moviesService.GetMoviesAsync(paginationParameters, movieFilters);
            return ToActionResult(result);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetMoviesDto>> GetMovie(int id)
        {
            var result = await moviesService.GetMovieByIdAsync(id);
            return ToActionResult(result);
        }

        [HttpGet("my-movies")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.MovieAdmin}")]
        public async Task<ActionResult<PagedResult<GetMoviesDto>>> GetMyMovies
            ([FromQuery] PaginationParameters paginationParameters)
        {
            var result = await moviesService.GetMyMoviesAsync(paginationParameters);
            return ToActionResult(result);
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.MovieAdmin}")]
        public async Task<IActionResult> PutMovie(int id, UpdateMovieDto movieDto)
        {
            if (id != movieDto.Id)
            {
                return BadRequest();
            }

            var result = await moviesService.UpdateMovieAsync(id, movieDto);
            return ToActionResult(result);
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.MovieAdmin}")]
        public async Task<ActionResult<GetMoviesDto>> PostMovie(CreateMovieDto movieDto)
        {
            var result = await moviesService.CreateMovieAsync(movieDto);
            if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

            return CreatedAtAction(nameof(GetMovie), new { id = result.Value!.Id }, result.Value);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.MovieAdmin}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var result = await moviesService.DeleteMovieAsync(id);
            return ToActionResult(result);
        }
    }
}
