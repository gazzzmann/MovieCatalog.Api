using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieCatalog.Api.Common.Constants;

namespace MovieCatalog.Api.Domain.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "8e653da4-c1c7-4c7d-b0bc-0a62fd6bd4a1",
                    Name = AppRoles.Admin,
                    NormalizedName = AppRoles.Admin.ToUpper(),
                    ConcurrencyStamp = "8f011ee2-e9e7-4351-bbbb-72dd8fb9a611"
                },
                new IdentityRole
                {
                    Id = "06209a3b-00eb-442b-b487-45ab183f3729",
                    Name = AppRoles.User,
                    NormalizedName = AppRoles.User.ToUpper(),
                    ConcurrencyStamp = "6fb1918e-5dde-477f-84cc-d951c30efddb"
                },
                 new IdentityRole
                 {
                     Id = "11209a3b-01eb-441b-b187-45ab183f3730",
                     Name = AppRoles.MovieAdmin,
                     NormalizedName = AppRoles.MovieAdmin.ToUpper(),
                     ConcurrencyStamp = "7cb1918e-6cde-488j-84hc-A251c30ef55y"
                 }
            );
        }
    }
}
