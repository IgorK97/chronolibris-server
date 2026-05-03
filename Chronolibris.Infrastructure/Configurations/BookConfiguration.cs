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
    public class BookConfiguration :IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasMany(b => b.Persons)
                .WithMany(b => b.Books)
                .UsingEntity<BookParticipation>();

            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.ToTable("books", t =>
            {
                t.HasCheckConstraint(
                    "CK_books_title_alnum",
                    "title ~ '[[:alnum:]]'");

                t.HasCheckConstraint(
                    "CK_books_description_min_length",
                    "LENGTH(description) >= 120");

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

            //builder.HasMany(b => b.Selections)
            //    .WithMany(s => s.Books)
            //    .UsingEntity("book_selection");

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

            DateTime dt = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc);


            builder.HasData(
                new Book
                {
                    Id = 1,
                    CountryId = 1,
                    CoverPath = "BuddismHistory/BuddismJapanGrig/MainFile.png",
                    CreatedAt = dt,
                    Description = "Монография является первой в отечественной литературе попыткой...",
                    IsAvailable = true,
                    LanguageId = 2,
                    //RatingsCount = 0,
                    //ReviewsCount = 0,
                    Title = "Буддизм в Японии",
                    Year = 1993,
                    PublisherId = 2,
                    IsReviewable = true,
                },
                new Book
                {
                    Id = 2,
                    //AverageRating = 0,
                    CountryId = 1,
                    CoverPath = "EconomicHistory/StructureBrodel/MainFile.png",
                    CreatedAt = dt,
                    Description = "Это — второе крупное исследование Ф. Броделя...",
                    //FilePath = "EconomicHistory/StructureBrodel/MainFile.epub",
                    IsAvailable = true,
                    //IsFragment = false,
                    LanguageId = 2,
                    //RatingsCount = 0,
                    //ReviewsCount = 0,
                    Title = "Структуры повседневности: возможное и невозможное",
                    Year = 1986,
                    PublisherId = 1,
                    IsReviewable = true,
                }
            );

        }
    }
}
