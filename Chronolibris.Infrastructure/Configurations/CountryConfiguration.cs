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
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_countries_name_not_empty",
                "LENGTH(TRIM(name))>0"));
            
            builder.HasData(
                new Country { Id = 1, Name = "Россия" },
                new Country { Id = 2, Name = "СССР" },
                new Country { Id = 3, Name = "Российская империя" },
                new Country { Id = 4, Name = "США" },
                new Country { Id = 5, Name = "Франция" }
            );
        }
    }
}
