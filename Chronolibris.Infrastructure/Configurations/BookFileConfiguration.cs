using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class BookFileConfiguration : IEntityTypeConfiguration<BookFile>
    {
        public void Configure(EntityTypeBuilder<BookFile> builder)
        {
            builder.ToTable("book_files");
            builder.HasOne(bf => bf.BookFileStatus)
                .WithMany(bs => bs.BookFiles)
                .HasForeignKey(b => b.BookFileStatusId)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasIndex(bf => new { bf.BookId, bf.IsReadable })
                .IsUnique()
                .HasFilter("\"is_readable\" = true");

            builder
                .ToTable(t => t.HasCheckConstraint("ck_book_files_readable_format",
                    "NOT (\"is_readable\" = true) OR (\"format_id\" IN (1, 2))"));
        }
    }
}
