namespace MovieCatalog.Api.Domain;

public class Genre
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public IList<Movie> Movies { get; set; } = [];
}
