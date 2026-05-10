using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class ReadingProgressConfiguration : IEntityTypeConfiguration<ReadingProgress>
    {
        public void Configure(EntityTypeBuilder<ReadingProgress> builder)
        {
            builder.HasOne<User>() 
               .WithMany()     
               .HasForeignKey(b => b.UserId) 
               .HasPrincipalKey(u => u.Id);

            builder.HasIndex(rp => new { rp.UserId, rp.BookFileId }).IsUnique();
        }
    }
}
