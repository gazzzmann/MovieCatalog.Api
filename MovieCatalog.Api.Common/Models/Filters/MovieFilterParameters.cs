namespace MovieCatalog.Api.Common.Models.Filters;

public class MovieFilterParameters : BaseFilterParameters
{
    public string? Genre { get; set; }
    public double? MinRating { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Director { get; set; }
}
