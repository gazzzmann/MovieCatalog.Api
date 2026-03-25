using MovieCatalog.Api.Application.DTOs.Movie;

public record GetGenresDto(
    int Id,
    string Name
);

public record GetGenreDto(
    int Id,
    string Name,
    List<GetGenreMovieDto> Movies
); 