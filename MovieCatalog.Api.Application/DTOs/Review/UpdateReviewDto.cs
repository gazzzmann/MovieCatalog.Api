using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Api.Application.DTOs.Review;

public class UpdateReviewDto
{
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(500)]
    public string Comment { get; set; } = string.Empty;
}
