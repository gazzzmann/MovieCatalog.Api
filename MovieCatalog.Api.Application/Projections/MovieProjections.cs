using System.Linq.Expressions;
using MovieCatalog.Api.Application.DTOs.Movie;
using MovieCatalog.Api.Domain;

namespace MovieCatalog.Api.Application.Projections;

public static class MovieProjections
{
    public static Expression<Func<Movie, GetMoviesSlimDto>> ToSlimDto =>
        m => new GetMoviesSlimDto(
            m.Id,
            m.Title,
            m.ReleaseYear,
            m.Director,
            m.Genre!.Name,
            Math.Round(m.Reviews.Select(r => (double?)r.Rating).Average() ?? 5, 1)
        );

    public static Expression<Func<Movie, GetMoviesDto>> ToDto =>
        m => new GetMoviesDto(
            m.Id,
            m.Title,
            m.Description,
            m.ReleaseYear,
            m.Director,
            m.GenreId,
            m.Genre!.Name,
            Math.Round(m.Reviews.Select(r => (double?)r.Rating).Average() ?? 5, 1),
            m.Reviews.Count(),
            m.BuyPrice,
            m.RentPrice
        );
}