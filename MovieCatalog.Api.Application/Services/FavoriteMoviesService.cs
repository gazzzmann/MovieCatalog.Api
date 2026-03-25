using Microsoft.EntityFrameworkCore;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Favorite;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Extensions;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;
using MovieCatalog.Api.Domain;

namespace MovieCatalog.Api.Application.Services;

public class FavoriteService(
    MovieCatalogDbContext context,
    IUsersService usersService) : IFavoriteService
{
    public async Task<Result<PagedResult<FavoriteMovieDto>>> GetFavoritesAsync
        (PaginationParameters paginationParameters)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<PagedResult<FavoriteMovieDto>>.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var favorites = await context.FavoriteMovies
              .Where(x => x.UserId == userId)
              .Select(x => new FavoriteMovieDto
              {
                  MovieId = x.Movie!.Id,
                  Title = x.Movie.Title,
                  Director = x.Movie.Director,
                  ReleaseYear = x.Movie.ReleaseYear,
                  Genre = x.Movie.Genre!.Name
              })
              .ToPagedResultAsync(paginationParameters);


        return Result<PagedResult<FavoriteMovieDto>>.Success(favorites);
    }
    public async Task<Result> AddFavoriteAsync(int movieId)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var movieExists = await context.Movies.AnyAsync(m => m.Id == movieId);

        if (!movieExists)
        {
            return Result.NotFound(
                new Error(ErrorCodes.NotFound, $"Movie with ID {movieId} was not found"));
        }

        var exists = await context.FavoriteMovies
            .AnyAsync(x => x.UserId == userId && x.MovieId == movieId);

        if (exists)
        {
            return Result.Failure(
                new Error(ErrorCodes.Validation, "Movie already in favorites"));
        }

        var favorite = new FavoriteMovie
        {
            UserId = userId,
            MovieId = movieId
        };

        context.FavoriteMovies.Add(favorite);
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> RemoveFavoriteAsync(int movieId)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var favorite = await context.FavoriteMovies
            .FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);

        if (favorite is null)
        {
            return Result.NotFound(
                new Error(ErrorCodes.NotFound, "Favorite movie not found"));
        }

        context.FavoriteMovies.Remove(favorite);
        await context.SaveChangesAsync();

        return Result.Success();
    }

}