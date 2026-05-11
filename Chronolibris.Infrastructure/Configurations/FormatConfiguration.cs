using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class FormatConfiguration :IEntityTypeConfiguration<Format>
    {
        public void Configure(EntityTypeBuilder<Format> builder)
        {
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("ck_formats_name_clean",
                    "name = LOWER(TRIM(name)) AND LENGTH(TRIM(name)) > 0");
            });

            builder.HasIndex(f => f.Name).IsUnique();


            builder.HasData(
                new Format
                {
                    Id = 1,
                    Name = "fb2"
                },
                new Format
                {
                    Id = 2,
                    Name = "epub"
                }
            );
        }
    }
}
