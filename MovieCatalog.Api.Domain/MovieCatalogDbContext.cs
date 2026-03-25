using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MovieCatalog.Api.Domain;

public partial class MovieCatalogDbContext(DbContextOptions<MovieCatalogDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<MovieAdmin> MovieAdmins { get; set; }
    public DbSet<FavoriteMovie> FavoriteMovies { get; set; }
    public DbSet<MovieTransaction> MovieTransactions { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}