using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class ReviewStatusConfiguration : IEntityTypeConfiguration<ReviewStatus>
    {
        public void Configure(EntityTypeBuilder<ReviewStatus> builder)
        {
            builder.HasData(
                new ReviewStatus{
                    Id=1,
                    Name= "На проверке",

                },
                new ReviewStatus { Id=2,
                    Name= "Опубликован",
                },
                new ReviewStatus { Id=3,
                Name="Отклонен"},
                new ReviewStatus { Id=4,
                Name="Удален"}
            );
        }
    }
}
