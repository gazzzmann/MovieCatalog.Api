namespace MovieCatalog.Api.Domain;

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int ReleaseYear { get; set; }
    public required string Director { get; set; }
    public decimal BuyPrice { get; set; }
    public decimal RentPrice { get; set; }
    public int GenreId { get; set; }
    public Genre? Genre { get; set; }

    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<FavoriteMovie> FavoritedBy { get; set; } = [];
    public ICollection<MovieTransaction> Transactions { get; set; } = [];
}
