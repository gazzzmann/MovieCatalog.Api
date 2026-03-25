using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Api.Application.DTOs.Movie;

public class CreateMovieDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    [Range(1888, 2100)]
    public int ReleaseYear { get; set; }
    [Required]
    public string Director { get; set; } = string.Empty;
    [Required]
    public int GenreId { get; set; }

    [Required]
    [Range(0.01, 5000)]
    public decimal BuyPrice { get; set; }

    [Required]
    [Range(0.01, 5000)]
    public decimal RentPrice { get; set; }
}

