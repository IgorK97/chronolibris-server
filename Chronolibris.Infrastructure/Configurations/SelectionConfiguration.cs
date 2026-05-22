using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class SelectionConfiguration : IEntityTypeConfiguration<Selection>
    {
        public void Configure(EntityTypeBuilder<Selection> builder)
        {
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne<User>()
            //    .WithMany()
            //    .HasForeignKey(s => s.UpdatedBy);

            builder.Property(s => s.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            //builder.ToTable("selections", t =>
            //{
            //    t.HasCheckConstraint(
            //        "CK_selections_updated",
            //        "(updated_at is NULL AND updated_by is NULL) OR (updated_at is NOT NULL AND updated_by is NOT NULL)");
            //});

            DateTime dt = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc);


            builder.HasData(
                new Selection
                {
                    Id = 1,
                    CreatedAt = dt,
                    Description = "",
                    IsActive = true,
                    Name = "Экономическая история",
                    CreatedBy = 1,
                    //SelectionTypeId=3
                },
                new Selection
                {
                    Id = 2,
                    CreatedAt = dt,
                    Description = "",
                    IsActive = true,
                    Name = "История культуры",
                    //SelectionTypeId = 3
                    CreatedBy = 1,

                },
                new Selection
                {
                    Id = 3,
                    CreatedAt = dt,
                    Description = "",
                    IsActive = true,
                    Name = "История мира",
                    //SelectionTypeId = 3
                    CreatedBy = 1,

                }
            );
        }
    }
}
