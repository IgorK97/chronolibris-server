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
    public class ContentConfiguration :IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            //builder.HasMany(c => c.Books)
            //    .WithMany(b => b.Contents)
            //    .UsingEntity<ContentBook>();

            builder.HasMany(c => c.Tags)
                .WithMany(t => t.Contents)
                .UsingEntity(j => j.ToTable("content_tags"));

            builder.HasOne(c => c.ParentContent)
                .WithMany()
                .HasForeignKey(c => c.ParentContentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ContentType)
                .WithMany(ct =>ct.Contents)
                .HasForeignKey(c => c.ContentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Persons)
                .WithMany(p => p.Contents)
                .UsingEntity<ContentParticipation>();

            //builder.HasMany(c => c.Themes)
            //    .WithMany(th => th.Contents)
            //    .UsingEntity("content_theme");

            builder.HasMany(c => c.Themes)
                .WithMany(th => th.Contents)
                .UsingEntity(
                    r => r.HasOne(typeof(Theme))
                          .WithMany()
                          .HasForeignKey("theme_id"),
                    l => l.HasOne(typeof(Content))
                          .WithMany()
                          .HasForeignKey("content_id"),
                    j => j.ToTable("content_theme")
                );




            DateTime dt = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc);


            builder.HasData(
                new Content
                {
                    Id = 1,
                    CountryId = 1, // Россия
                    CreatedAt = dt,
                    Description = "Монография является первой в отечественной литературе попыткой проследить процесс становления японского буддизма...",
                    LanguageId = 2, // Русский
                    Position = 0,
                    Title = "Буддизм в Японии",
                    Year = 1993,
                    ContentTypeId = 20, // Монография
                },
                new Content
                {
                    Id = 2,
                    CountryId = 5, // Франция
                    CreatedAt = dt,
                    Description = "Это — второе крупное исследование Ф. Броделя. Первое — «Средиземное море и мир Средиземноморья в эпоху Филиппа II»...",
                    LanguageId = 2, // Русский
                    Position = 0,
                    Title = "Структуры повседневности: возможное и невозможное",
                    Year = 1979,
                    ContentTypeId = 19, // Историческое исследование
                }
            );
        }
    }
}
