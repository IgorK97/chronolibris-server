using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class BookFileConfiguration : IEntityTypeConfiguration<BookFile>
    {
        public void Configure(EntityTypeBuilder<BookFile> builder)
        {
            builder.ToTable("book_files", t =>
            {
                t.HasCheckConstraint("CK_book_files_original_size_positive",
                    "original_size > 0");

                t.HasCheckConstraint("CK_book_files_stored_size_not_negative",
                    "stored_size>=0");

                t.HasCheckConstraint("ck_book_files_readable_format",
                    "NOT (\"is_readable\" = true) OR (\"format_id\" = 1)");

                t.HasCheckConstraint("ck_book_files_archive_rule",
                    "(status_id = 6 AND is_readable = true AND hidden_at IS NOT NULL) " +
                    "OR " +
                    "(status_id != 6 AND hidden_at IS NULL)");
                t.HasCheckConstraint(
                    "CK_book_files_hidden",
                    "(hidden_at = NULL AND status_id < 6) OR (hidden_at != NULL AND status_id >=6)");
                t.HasCheckConstraint(
                    "CK_book_files_deleted",
                    "(deleted_at = NULL AND status_id != 7) OR (deleted_at != NULL AND status_id = 7)");
                t.HasCheckConstraint(
                    "CK_book_files_completed",
                    "(completed_at = NULL AND status_id < 4) OR (completed_at != NULL AND status_id >= 4)");
                });

            builder.HasOne(bf => bf.BookFileStatus)
                .WithMany(bs => bs.BookFiles)
                .HasForeignKey(b => b.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(bf => bf.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.CreatedBy);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.HiddenBy);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.DeletedBy);

            //builder.Property(bf => bf.HistoricalText);

            builder
                .HasIndex(bf => new { bf.BookId, bf.FormatId, bf.HistoricalText })
                .IsUnique().HasFilter("status_id != 6").AreNullsDistinct(false);

            //builder
            //    .HasIndex(bf => new { bf.BookId, bf.IsReadable })
            //    .IsUnique()
            //    .HasFilter("\"is_readable\" = true");

            ////builder
            ////    .ToTable(t => t.HasCheckConstraint("ck_book_files_readable_format",
            ////        "NOT (\"is_readable\" = true) OR (\"format_id\" = 1)"));

            //builder
            //    .HasIndex(bf => new { bf.BookId, bf.FormatId })
            //    .IsUnique();
        }
    }
}
