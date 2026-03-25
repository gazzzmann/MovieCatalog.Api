using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieCatalog.Api.Domain.Configurations;

public class MovieAdminConfiguration : IEntityTypeConfiguration<MovieAdmin>
{
    public void Configure(EntityTypeBuilder<MovieAdmin> builder)
    {
        builder.HasKey(ma => ma.Id);

        builder.HasIndex(ma => new { ma.UserId, ma.MovieId })
               .IsUnique();
    }
}
