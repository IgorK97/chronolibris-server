using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class ShelfTypeConfiguration : IEntityTypeConfiguration<ShelfType>
    {
        public void Configure(EntityTypeBuilder<ShelfType> builder)
        {
            builder.HasData(
                new ShelfType { Id = 1, Code = "FAVORITES" },
                new ShelfType { Id = 2, Code = "READ" },
                new ShelfType { Id = 3, Code = "CUSTOM" }

            );
        }
    }
}
