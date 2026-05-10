using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class ReviewReactionsConfiguration : IEntityTypeConfiguration<ReviewReactions>
    {
        public void Configure(EntityTypeBuilder<ReviewReactions> builder)
        {
            builder.ToTable(rr=>rr.HasCheckConstraint("ck_review_reactions_reaction_type",
                "reaction_type IN (1, -1, 0)"));

            builder.HasOne<User>() 
                  .WithMany()     
                  .HasForeignKey(b => b.UserId) 
                  .HasPrincipalKey(u => u.Id);

            builder.HasIndex(r => new { r.UserId, r.ReviewId })
               .IsUnique();
        }
    }
}
