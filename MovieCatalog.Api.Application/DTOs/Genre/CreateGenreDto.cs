using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Api.Application.DTOs.Genre
{
    public class CreateGenreDto
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }
    }
}

