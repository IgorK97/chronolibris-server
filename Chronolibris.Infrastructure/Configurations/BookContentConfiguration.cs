using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class BookContentConfiguration : IEntityTypeConfiguration<BookContent>
    {
        public void Configure(EntityTypeBuilder<BookContent> builder)
        {

            //        modelBuilder.Entity<BookContent>()
            //.HasKey(bc => new { bc.BookId, bc.ContentId });

            //        modelBuilder.Entity<BookContent>()
            //            .HasOne(bc => bc.Book)
            //            .WithMany(b => b.BookContents)
            //            .HasForeignKey(bc => bc.BookId);

            //        modelBuilder.Entity<BookContent>()
            //            .HasOne(bc => bc.Content)
            //            .WithMany(c => c.BookContents)
            //            .HasForeignKey(bc => bc.ContentId);

            builder
            .HasKey(bc => new { bc.BookId, bc.ContentId });

            builder
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookContents)
                .HasForeignKey(bc => bc.BookId);

            builder
                .HasOne(bc => bc.Content)
                .WithMany(c => c.BookContents)
                .HasForeignKey(bc => bc.ContentId);

            builder.HasData(
                new BookContent
                {
                    BookId = 1,
                    ContentId = 1,
                },

                new BookContent
                {
                    BookId = 2,
                    ContentId = 2,
                }


            );
        }
    }
}
