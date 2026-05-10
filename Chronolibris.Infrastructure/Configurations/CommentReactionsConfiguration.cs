using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class CommentReactionsConfiguration : IEntityTypeConfiguration<CommentReactions>
    {
        public void Configure(EntityTypeBuilder<CommentReactions> builder)
        {
            builder.ToTable(rr => rr.HasCheckConstraint("ck_comment_reactions_reaction_type",
                "reaction_type IN (1, -1, 0)"));

            builder.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(b => b.UserId)
                  .HasPrincipalKey(u => u.Id);

            builder.HasIndex(r => new { r.UserId, r.CommentId })
               .IsUnique();
        }
    }
}
