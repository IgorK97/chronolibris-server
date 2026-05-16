using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class BookConfiguration :IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasMany(b => b.Persons)
                .WithMany(p => p.Books)
                .UsingEntity<BookParticipation>(
                            j => j.HasOne(bp => bp.Person)
                                   .WithMany(p=>p.BookParticipations)
                                   .HasForeignKey(bp => bp.PersonId)
                                   .OnDelete(DeleteBehavior.Restrict),
                            j => j.HasOne(bp => bp.Book)
                                   .WithMany(b=>b.Participations)
                                   .HasForeignKey(bp => bp.BookId)
                                   .OnDelete(DeleteBehavior.Cascade)
               );

            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(b => b.HasHistoricalVersions)
                .HasDefaultValue(true);

            builder.ToTable("books", t =>
            {
                t.HasCheckConstraint(
                    "CK_books_title_alnum",
                    "title ~ '[[:alnum:]]'");

                t.HasCheckConstraint(
                    "CK_books_description_min_length",
                    "LENGTH(TRIM(description)) >= 120");

                t.HasCheckConstraint(
                    "CK_books_year_range",
                    "year IS NULL OR (year>=-10000 AND year <= EXTRACT(YEAR FROM CURRENT_DATE)+1)");
            });

            builder.HasMany(b => b.Shelves)
                .WithMany(s => s.Books)
                .UsingEntity<BookShelf>(
                    l => l.HasOne(bs => bs.Shelf)
                          .WithMany(s => s.BookShelves)
                          .HasForeignKey(bs => bs.ShelfId),
                    r => r.HasOne(bs => bs.Book)
                          .WithMany(b => b.BookShelves)
                          .HasForeignKey(bs => bs.BookId),
                    j =>
                    {
                        j.ToTable("book_shelf");
                        j.HasKey(bs => new { bs.BookId, bs.ShelfId });

                        j.Property(bs => bs.AddedAt)
                         .HasDefaultValueSql("CURRENT_TIMESTAMP");
                    }
                );

            builder.HasMany(b => b.Selections)
                .WithMany(s => s.Books)
                .UsingEntity(
                    r => r.HasOne(typeof(Selection))
                          .WithMany()
                          .HasForeignKey("selection_id"),
                    l => l.HasOne(typeof(Book))
                          .WithMany()
                          .HasForeignKey("book_id"),
                    j => j.ToTable("book_selection")
                );

            builder.HasOne(b => b.Country)
                   .WithMany(c => c.Books)
                   .HasForeignKey(b => b.CountryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Language)
                   .WithMany(l => l.Books)
                   .HasForeignKey(b => b.LanguageId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Publisher)
                   .WithMany(p => p.Books)
                   .HasForeignKey(b => b.PublisherId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
