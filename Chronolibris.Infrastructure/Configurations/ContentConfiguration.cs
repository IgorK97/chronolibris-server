using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class ContentConfiguration :IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {

            builder.ToTable("contents", t =>
            {
                t.HasCheckConstraint("CK_contents_title_alnum",
                    "title ~ '[[:alnum:]]'");

                t.HasCheckConstraint("CK_contents_description_min_length",
                    "LENGTH(TRIM(description))>=120");

                t.HasCheckConstraint("CK_contents_years",
                    "(year_from is null AND year_to is null) " +
                    "OR (year_from >= -10000 AND year_from <= year_to AND year_to <= EXTRACT(YEAR FROM CURRENT_TIMESTAMP)+1)" +
                    "OR (year_from is null AND -10000 <= year_to AND year_to <= EXTRACT(YEAR FROM CURRENT_TIMESTAMP)+1)" +
                    "OR (year_to is null AND -10000 <= year_from AND year_from <= EXTRACT(YEAR FROM CURRENT_TIMESTAMP)+1)");
            });

            builder.HasMany(c => c.Tags)
                .WithMany(t => t.Contents)
                .UsingEntity<Dictionary<string, object>>("content_tags",
                j => j.HasOne<Tag>().WithMany().HasForeignKey("tags_id").OnDelete(DeleteBehavior.Restrict),
                j => j.HasOne<Content>().WithMany().HasForeignKey("contents_id").OnDelete(DeleteBehavior.Cascade));

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(c => c.ContentType)
                .WithMany(ct =>ct.Contents)
                .HasForeignKey(c => c.ContentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Persons)
                .WithMany(p => p.Contents)
                .UsingEntity<ContentParticipation>(
                j => j.HasOne(bp => bp.Person)
                                   .WithMany(p => p.ContentParticipations)
                                   .HasForeignKey(bp => bp.PersonId)
                                   .OnDelete(DeleteBehavior.Restrict),
                            j => j.HasOne(bp => bp.Content)
                                   .WithMany(b => b.Participations)
                                   .HasForeignKey(bp => bp.ContentId)
                                   .OnDelete(DeleteBehavior.Cascade)
                );


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

            builder.HasOne(b => b.Country)
                   .WithMany(c => c.Contents)
                   .HasForeignKey(b => b.CountryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Language)
                   .WithMany(l => l.Contents)
                   .HasForeignKey(b => b.LanguageId)
                   .OnDelete(DeleteBehavior.Restrict);

            DateTime dt = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Content
                {
                    Id = 1,
                    CountryId = 1,
                    CreatedAt = dt,
                    Description = "Монография является первой в отечественной литературе попыткой проследить процесс становления японского буддизма...",
                    LanguageId = 2,
                    //Position = 0,
                    Title = "Буддизм в Японии",
                    YearFrom = 1993,
                    YearTo = 1993,
                    ContentTypeId = 20,
                },
                new Content
                {
                    Id = 2,
                    CountryId = 5,
                    CreatedAt = dt,
                    Description = "Это — второе крупное исследование Ф. Броделя. Первое — «Средиземное море и мир Средиземноморья в эпоху Филиппа II»...",
                    LanguageId = 2,
                    //Position = 0,
                    Title = "Структуры повседневности: возможное и невозможное",
                    YearFrom = 1979,
                    YearTo = 1979,
                    ContentTypeId = 19,
                }
            );
        }
    }
}
