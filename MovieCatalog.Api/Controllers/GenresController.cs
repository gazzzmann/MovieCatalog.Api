using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Genre;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Domain;

namespace MovieCatalog.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GenresController(IGenresService genresService) : BaseApiController
{

    // GET: api/Genres
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetGenresDto>>> GetGenres
     ([FromQuery] PaginationParameters paginationParameters)
    {
        var result = await genresService.GetGenresAsync(paginationParameters);
        return ToActionResult(result);
    }

    // GET: api/Genres/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetGenreDto>> GetGenre(int id)
    {
        var result = await genresService.GetGenreByIdAsync(id);
        return ToActionResult(result);
    }

    // PUT: api/Genres/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.MovieAdmin}")]
    public async Task<IActionResult> PutGenre(int id, UpdateGenreDto genreDto)
    {
        var result = await genresService.UpdateGenreAsync(id, genreDto);
        return ToActionResult(result);
    }

    // POST: api/Genres
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.MovieAdmin}")]
    public async Task<ActionResult<Genre>> PostGenre(CreateGenreDto genreDto)
    {
        var result = await genresService.CreateGenreAsync(genreDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return CreatedAtAction("GetGenre", new { id = result.Value!.Id }, result.Value);
    }

    // DELETE: api/Genres/5
    [HttpDelete("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var result = await genresService.DeleteGenreAsync(id);
        return ToActionResult(result);
    }

}
