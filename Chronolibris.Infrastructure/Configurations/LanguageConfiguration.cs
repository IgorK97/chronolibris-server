using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chronolibris.Infrastructure.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.HasData(
                new Language { Id = 1, Name = "Английский", FtsConfiguration="english" },
                new Language { Id = 2, Name = "Русский", FtsConfiguration="russian" },
                new Language { Id = 3, Name = "Французский", FtsConfiguration="french" },
                new Language { Id = 4, Name = "Немецкий", FtsConfiguration="german" }
            );
        }
    }
}
