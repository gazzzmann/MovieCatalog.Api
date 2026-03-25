using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Api.Application.DTOs.Favorite;

public class AddFavoriteDto
{
    [Required]
    public int MovieId { get; set; }
}
