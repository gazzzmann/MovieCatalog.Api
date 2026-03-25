using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Api.Application.DTOs.Movie;

public class UpdateMovieDto : CreateMovieDto
{
    [Required]
    public int Id { get; set; }
}   
