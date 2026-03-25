namespace MovieCatalog.Api.Application.DTOs.Favorite;

public class FavoriteMovieDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public string Genre { get; set; } = string.Empty;
}
