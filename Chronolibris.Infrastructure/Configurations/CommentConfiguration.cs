using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            //builder.ToTable("comments", t =>
            //{
            //    t.HasCheckConstraint("CK_comments_text_min_length",
            //        "text !~ '^\\s*$'"
            //        );
            //});

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(u => u.Id);

            builder.HasOne(c => c.Book)
                .WithMany(b => b.Comments)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(c => new { c.BookId, c.CreatedAt });
            builder.HasIndex(c => c.ParentCommentId);
            //builder.HasIndex(c => c.UserId);
        }
    }
}
