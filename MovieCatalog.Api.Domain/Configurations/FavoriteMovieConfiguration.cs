using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieCatalog.Api.Domain.Configurations;

public class FavoriteMovieConfiguration : IEntityTypeConfiguration<FavoriteMovie>
{
    public void Configure(EntityTypeBuilder<FavoriteMovie> builder)
    {
        builder.HasKey(f => new { f.UserId, f.MovieId });

        builder.HasOne(f => f.User)
               .WithMany(u => u.FavoriteMovies)
               .HasForeignKey(f => f.UserId);

        builder.HasOne(f => f.Movie)
               .WithMany(m => m.FavoritedBy)
               .HasForeignKey(f => f.MovieId);
    }
}
