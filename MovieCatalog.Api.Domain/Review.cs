namespace MovieCatalog.Api.Domain;

public class Review
{
    public int Id { get; set; }

    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    public required int MovieId { get; set; }
    public Movie? Movie { get; set; }

    public string Comment { get; set; } = string.Empty;
    public required int Rating { get; set; }
}
