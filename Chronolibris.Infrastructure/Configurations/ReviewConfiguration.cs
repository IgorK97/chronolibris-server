using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            builder.ToTable(r => r.HasCheckConstraint("ck_review_rating", 
                "score >=0.0 AND score<=5.0"));

            builder.HasOne<User>() 
                   .WithMany()     
                   .HasForeignKey(b => b.UserId)
                   .HasPrincipalKey(u => u.Id);

            builder.HasOne<ReviewStatus>(r => r.ReviewStatus)
               .WithMany(rs => rs.Reviews)
               .HasForeignKey(b => b.ReviewStatusId)
               .HasPrincipalKey(u => u.Id);

            builder.HasIndex(r => new { r.UserId, r.BookId })
               .IsUnique();

            //builder.HasCheckConstraint("CK_Review_Rating", "[Score] >=0.0 AND [Score]<=5.0");
        }
    }
}
