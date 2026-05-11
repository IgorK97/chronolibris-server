using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable(r =>
            {
                r.HasCheckConstraint("ck_review_rating",
                    "score IN (1,2,3,4,5)");
            }
            );

            builder.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne<User>() 
                   .WithMany()     
                   .HasForeignKey(b => b.UserId)
                   .HasPrincipalKey(u => u.Id);

            builder.HasIndex(r => new { r.UserId, r.BookId })
               .IsUnique().HasFilter("is_deleted = false");
               //.HasFilter("\"review_status_id\" != 4");
        }
    }
}