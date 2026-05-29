using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class ModerationTaskConfiguration : IEntityTypeConfiguration<ModerationTask>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ModerationTask> builder)
        {

            builder.HasOne(mt => mt.Status)
                .WithMany(s => s.Tasks)
                .HasForeignKey(mt => mt.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mt => mt.Book)
                .WithMany()
                .HasForeignKey(mt => mt.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mt => mt.Comment)
                .WithMany()
                .HasForeignKey(mt => mt.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mt => mt.Review)
                .WithMany()
                .HasForeignKey(mt => mt.ReviewId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("moderation_tasks",
                builder => builder.HasCheckConstraint("ck_moderation_task_target", "book_id IS NOT NULL OR review_id IS NOT NULL OR comment_id IS NOT NULL"));

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(mt => mt.ModeratedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(mt => mt.StartedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(mt => new { mt.BookId })
                .IsUnique()
                .HasFilter("status_id = 2")
                .HasDatabaseName("ix_moderation_tasks_one_book_only");

            builder.HasIndex(mt => new { mt.CommentId })
                .IsUnique()
                .HasFilter("status_id = 2")
                .HasDatabaseName("ix_moderation_tasks_one_comment_only");

            builder.HasIndex(mt => new { mt.ReviewId })
                .IsUnique()
                .HasFilter("status_id = 2")
                .HasDatabaseName("ix_moderation_tasks_one_review_only");
        }
    }
}
