using Microsoft.EntityFrameworkCore;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Genre;
using MovieCatalog.Api.Application.DTOs.Movie;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Extensions;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;
using MovieCatalog.Api.Domain;

namespace MovieCatalog.Api.Application.Services;

public class GenresService(MovieCatalogDbContext context) : IGenresService
{
    public async Task<Result<PagedResult<GetGenresDto>>> GetGenresAsync
        (PaginationParameters paginationParameters)
    {
        var genres = await context.Genres
            .Select(g => new GetGenresDto(
                g.Id,
                g.Name
            ))
             .ToPagedResultAsync(paginationParameters);
        return Result<PagedResult<GetGenresDto>>.Success(genres);
    }

    public async Task<Result<GetGenreDto>> GetGenreByIdAsync(int id)
    {
        var genre = await context.Genres
            .Where(g => g.Id == id)
            .Select(g => new GetGenreDto(
                g.Id,
                g.Name,
                g.Movies
                    .Select(m => new GetGenreMovieDto(
                        m.Id,
                        m.Title,
                        m.ReleaseYear,
                        m.Director,
                        Math.Round(
                            m.Reviews.Select(r => (double?)r.Rating).Average() ?? 5,
                            1)
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync();

        return genre is null
            ? Result<GetGenreDto>.NotFound()
            : Result<GetGenreDto>.Success(genre);
    }

    public async Task<Result<GetGenreDto>> CreateGenreAsync(CreateGenreDto genreDto)
    {
        try
        {
            var existingGenre = await GenreExistsAsync(genreDto.Name);
            if (existingGenre)
            {
                return Result<GetGenreDto>.BadRequest
                    (new Error(ErrorCodes.Conflict,
                    $"{genreDto.Name} genre already exists."));
            }

            var genre = new Genre
            {
                Name = genreDto.Name
            };

            context.Genres.Add(genre);
            await context.SaveChangesAsync();

            var createdDto = new GetGenreDto(
                genre.Id,
                genre.Name,
                []
            );

            return Result<GetGenreDto>.Success(createdDto);
        }
        catch (Exception)
        {
            return Result<GetGenreDto>.Failure();
        }
    }

    public async Task<Result> UpdateGenreAsync(int id, UpdateGenreDto genreDto)
    {
        try
        {
            if (id != genreDto.Id)
            {
                return Result.BadRequest
                    (new Error(ErrorCodes.BadRequest,
                    "ID in URL does not match ID in body."));
            }

            var genre = await context.Genres.FindAsync(id);
            if (genre is null)
            {
                return Result.NotFound
                    (new Error(ErrorCodes.NotFound,
                    $"Genre with ID {id} not found."));
            }

            genre.Name = genreDto.Name;

            context.Genres.Update(genre);
            await context.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }

    }

    public async Task<Result> DeleteGenreAsync(int id)
    {
        try
        {
            var genre = await context.Genres.FindAsync(id);
            if (genre is null)
            {
                return Result.NotFound
                    (new Error("Not Found",
                    $"Genre with ID {id} not found."));
            }
            context.Genres.Remove(genre);
            await context.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure();
        }
    }

    public async Task<bool> GenreExistsAsync(int id)
    {
        return await context.Genres.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> GenreExistsAsync(string name)
    {
        return await context.Genres
            .AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
    }

}

