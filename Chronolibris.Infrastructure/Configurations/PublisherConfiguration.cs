using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder.HasData(
                new Publisher { Id = 1, CountryId = 2, Name = "Прогресс", Description = "", CreatedAt = new DateTime(2026, 4, 9) },
                new Publisher { Id = 2, CountryId = 1, Name = "Восточная литература", Description = "", CreatedAt = new DateTime(2026, 4, 9) }
            );
        }
    }
}
