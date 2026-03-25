using Microsoft.EntityFrameworkCore;
using MovieCatalog.Api.Application.Contracts;
using MovieCatalog.Api.Application.DTOs.Movie;
using MovieCatalog.Api.Application.Projections;
using MovieCatalog.Api.Common.Constants;
using MovieCatalog.Api.Common.Models.Extensions;
using MovieCatalog.Api.Common.Models.Filters;
using MovieCatalog.Api.Common.Models.Paging;
using MovieCatalog.Api.Common.Results;
using MovieCatalog.Api.Domain;

namespace MovieCatalog.Api.Application.Services;

public class MoviesService(
     MovieCatalogDbContext context,
     IGenresService genresService,
     IUsersService usersService) : IMoviesService
{
    public async Task<Result<PagedResult<GetMoviesSlimDto>>> GetMoviesAsync(
        PaginationParameters paginationParameters,
        MovieFilterParameters movieFilters)
    {
        var query = context.Movies
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(movieFilters.Genre))
        {
            var genre = movieFilters.Genre.Trim().ToLower();
            query = query.Where(m => m.Genre != null &&
                                     m.Genre.Name.ToLower() == genre);
        }

  
        if (movieFilters.MinRating.HasValue)
        {
            query = query.Where(m =>
                (m.Reviews.Select(r => (double?)r.Rating).Average() ?? 5)
                >= movieFilters.MinRating.Value);
        }

        if (movieFilters.ReleaseYear.HasValue)
        {
            query = query.Where(m => m.ReleaseYear == movieFilters.ReleaseYear.Value);
        }

        if (!string.IsNullOrWhiteSpace(movieFilters.Director))
        {
            var director = movieFilters.Director.Trim();
            query = query.Where(m =>
                EF.Functions.Like(m.Director, $"%{director}%"));
        }

        if (!string.IsNullOrWhiteSpace(movieFilters.Search))
        {
            var search = movieFilters.Search.Trim();

            query = query.Where(m =>
                EF.Functions.Like(m.Title, $"%{search}%") ||
                EF.Functions.Like(m.Director, $"%{search}%") ||
                EF.Functions.Like(m.Description, $"%{search}%"));
        }

        query = movieFilters.SortBy?.ToLower() switch
        {
            "title" => movieFilters.SortOrder
                ? query.OrderByDescending(m => m.Title)
                : query.OrderBy(m => m.Title),

            "rating" => movieFilters.SortOrder
                ? query.OrderByDescending(m =>
                    m.Reviews.Select(r => (double?)r.Rating).Average() ?? 5)
                : query.OrderBy(m =>
                    m.Reviews.Select(r => (double?)r.Rating).Average() ?? 5),

            "releaseyear" => movieFilters.SortOrder
                ? query.OrderByDescending(m => m.ReleaseYear)
                : query.OrderBy(m => m.ReleaseYear),

            _ => query.OrderBy(m => m.Title)
        };

        var movies = await query
            .Select(MovieProjections.ToSlimDto)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetMoviesSlimDto>>.Success(movies);
    }

    public async Task<Result<GetMoviesDto>> GetMovieByIdAsync(int id)
    {
        var movie = await context.Movies
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(MovieProjections.ToDto)
            .FirstOrDefaultAsync();

        if (movie is null)
        {
            return Result<GetMoviesDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Movie with ID {id} was not found"));
        }

        return Result<GetMoviesDto>.Success(movie);
    }

    public async Task<Result<PagedResult<GetMoviesDto>>> GetMyMoviesAsync(
        PaginationParameters paginationParameters)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<PagedResult<GetMoviesDto>>.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var movies = await context.MovieAdmins
            .AsNoTracking()
            .Where(ma => ma.UserId == userId)
            .Select(ma => ma.Movie!)
            .Select(MovieProjections.ToDto)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetMoviesDto>>.Success(movies);
    }

    public async Task<Result<GetMoviesDto>> CreateMovieAsync(CreateMovieDto movieDto)
    {
        var userId = usersService.UserId;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result<GetMoviesDto>.Failure(
                new Error(ErrorCodes.Validation, "User not authenticated"));
        }

        var genreExists = await genresService.GenreExistsAsync(movieDto.GenreId);

        if (!genreExists)
        {
            return Result<GetMoviesDto>.Failure(
                new Error(ErrorCodes.NotFound,
                $"Genre with ID {movieDto.GenreId} was not found"));
        }

        var movie = new Movie
        {
            Title = movieDto.Title,
            Description = movieDto.Description,
            ReleaseYear = movieDto.ReleaseYear,
            Director = movieDto.Director,
            GenreId = movieDto.GenreId,
            BuyPrice = movieDto.BuyPrice,
            RentPrice = movieDto.RentPrice
        };

        context.Movies.Add(movie);

        context.MovieAdmins.Add(new MovieAdmin
        {
            Movie = movie,
            UserId = userId
        });

        await context.SaveChangesAsync();

        var createdMovie = await context.Movies
            .AsNoTracking()
            .Where(m => m.Id == movie.Id)
            .Select(MovieProjections.ToDto)
            .FirstAsync();

        return Result<GetMoviesDto>.Success(createdMovie);
    }

    public async Task<Result> UpdateMovieAsync(int id, UpdateMovieDto movieDto)
    {
        if (id != movieDto.Id)
        {
            return Result.BadRequest(
                new Error(ErrorCodes.BadRequest,
                "ID in URL does not match request body"));
        }

        var userId = usersService.UserId;

        // Ownership check
        var isOwner = await context.MovieAdmins
            .AnyAsync(ma => ma.MovieId == id && ma.UserId == userId);

        if (!isOwner)
        {
            return Result.Failure(
                new Error(ErrorCodes.Forbid, "You do not own this movie"));
        }

        var movie = await context.Movies.FindAsync(id);

        if (movie is null)
        {
            return Result.NotFound(
                new Error(ErrorCodes.NotFound,
                $"Movie with ID {id} was not found"));
        }

        var genreExists = await genresService.GenreExistsAsync(movieDto.GenreId);

        if (!genreExists)
        {
            return Result.Failure(
                new Error(ErrorCodes.NotFound,
                $"Genre with ID {movieDto.GenreId} was not found"));
        }

        movie.Title = movieDto.Title;
        movie.Description = movieDto.Description;
        movie.ReleaseYear = movieDto.ReleaseYear;
        movie.Director = movieDto.Director;
        movie.GenreId = movieDto.GenreId;
        movie.BuyPrice = movieDto.BuyPrice;
        movie.RentPrice = movieDto.RentPrice;

        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteMovieAsync(int id)
    {
        var userId = usersService.UserId;

        //Ownership check
        var isOwner = await context.MovieAdmins
            .AnyAsync(ma => ma.MovieId == id && ma.UserId == userId);

        if (!isOwner)
        {
            return Result.Failure(
                new Error(ErrorCodes.Forbid, "You do not own this movie"));
        }

        var deleted = await context.Movies
            .Where(m => m.Id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0)
        {
            return Result.NotFound(
                new Error(ErrorCodes.NotFound,
                $"Movie with ID {id} was not found"));
        }

        return Result.Success();
    }

    public async Task<bool> MovieExistsAsync(int id)
    {
        return await context.Movies
            .AsNoTracking()
            .AnyAsync(m => m.Id == id);
    }
}