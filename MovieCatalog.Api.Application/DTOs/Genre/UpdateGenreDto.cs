using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Api.Application.DTOs.Genre
{
    public class UpdateGenreDto : CreateGenreDto
    {
        [Required]
        public required int Id { get; set; }
    }
}
