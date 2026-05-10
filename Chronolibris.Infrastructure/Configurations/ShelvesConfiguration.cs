using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class ShelvesConfiguration : IEntityTypeConfiguration<Shelf>
    {
        public void Configure(EntityTypeBuilder<Shelf> builder)
        {

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_shelves_name_not_empty",
                "LENGTH(TRIM(name))>0"));

            builder.HasOne<User>() 
                   .WithMany()     
                   .HasForeignKey(b => b.UserId) 
                   .HasPrincipalKey(u => u.Id);

            builder
            .HasIndex(s => new { s.UserId, s.ShelfTypeId })
            .HasDatabaseName("IX_Shelves_UserId_ShelfTypeId");
        }
    }
}
