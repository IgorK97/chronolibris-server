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
    public class SelectionConfiguration : IEntityTypeConfiguration<Selection>
    {
        public void Configure(EntityTypeBuilder<Selection> builder)
        {
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

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

                },
                new Selection
                {
                    Id = 4,
                    CreatedAt = dt,
                    Description = "",
                    IsActive = true,
                    Name = "Новое",
                    //SelectionTypeId = 1
                    CreatedBy=1,
                },
                new Selection {                     
                    Id = 5,
                    CreatedAt = dt,
                    Description = "",
                    IsActive = true,
                    Name = "Часто читают",
                    //SelectionTypeId = 2
                    CreatedBy=1,
                }
            );
        }
    }
}
