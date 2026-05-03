using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class BookFragmentConfiguration : IEntityTypeConfiguration<BookFragment>
    {
        public void Configure(EntityTypeBuilder<BookFragment> builder)
        {
            builder.Property(bchunk => bchunk.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.ToTable("book_fragments", t =>
            {
                t.HasCheckConstraint("CK_book_fragments_position_positive",
                "position >= 0");

                t.HasCheckConstraint("CK_book_fragments_start_end_pos_positive",
                    "end_pos >= start_pos AND start_pos >= 0");
            });

            builder
                .HasIndex(f => new { f.BookFileId, f.Position })
                .IsUnique();
        }
    }
}
