using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class FormatConfiguration :IEntityTypeConfiguration<Format>
    {
        public void Configure(EntityTypeBuilder<Format> builder)
        {
            builder.HasData(
                new Format
                {
                    Id = 1,
                    Name = "fb2"
                },
                new Format
                {
                    Id = 2,
                    Name = "epub"
                }
                //new Format
                //{
                //    Id = 3,
                //    Name = "pdf"
                //},
                //new Format 
                //{ 
                //    Id=4,
                //    Name="txt"
                //}
            );
        }
    }
}
