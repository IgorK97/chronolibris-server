using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_publishers_name_not_empty",
                "LENGTH(TRIM(name))>0"));

            builder.HasData(
                new Publisher { Id = 1, Name = "Прогресс", Description = "", CreatedAt = new DateTime(2026, 4, 9) },
                new Publisher { Id = 2, Name = "Восточная литература", Description = "", CreatedAt = new DateTime(2026, 4, 9) }
            );
        }
    }
}
