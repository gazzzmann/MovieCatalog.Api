using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieCatalog.Api.Domain;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;   
    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public string FullName => $"{LastName} {FirstName}";
    public ICollection<FavoriteMovie> FavoriteMovies { get; set; } = [];
    public ICollection<MovieAdmin> MovieAdmins { get; set; } = [];
    public decimal WalletBalance { get; set; }
}
