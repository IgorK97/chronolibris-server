using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_languages_name_not_empty",
                "LENGTH(TRIM(name))>0"));

            builder.HasData(
                new Language { Id = 1, Name = "Английский" },
                new Language { Id = 2, Name = "Русский" },
                new Language { Id = 3, Name = "Французский"},
                new Language { Id = 4, Name = "Немецкий" }
            );
        }
    }
}
