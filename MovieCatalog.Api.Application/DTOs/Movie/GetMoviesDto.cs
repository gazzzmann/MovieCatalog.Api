namespace MovieCatalog.Api.Application.DTOs.Movie;

public record GetMoviesDto(
    int Id,
    string Title,
    string Description,
    int ReleaseYear,
    string Director,
    int GenreId,
    string Genre,
    double AverageRating,
    int ReviewsCount,
    decimal BuyPrice,
    decimal RentPrice
);

public record GetMoviesSlimDto(
    int Id,
    string Title,
    int ReleaseYear,
    string Director,
    string Genre,
    double AverageRating
);

public record GetGenreMoviesSlimDto(
    int Id,
    string Title,
    int ReleaseYear,
    string Director
);

public record GetGenreMovieDto(
    int Id,
    string Title,
    int ReleaseYear,
    string Director,
    double AverageRating
);