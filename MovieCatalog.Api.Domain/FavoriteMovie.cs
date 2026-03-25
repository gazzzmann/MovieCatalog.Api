namespace MovieCatalog.Api.Domain;

public class FavoriteMovie
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int MovieId { get; set; }
    public Movie? Movie { get; set; }
}