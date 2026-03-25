using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieCatalog.Api.Domain.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review> 
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => new { r.UserId, r.MovieId })
               .IsUnique();

        builder.Property(r => r.Comment)
               .HasMaxLength(500);

        builder.Property(r => r.Rating)
               .IsRequired();
    }
}
