using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class ReportReasonTypeConfiguration : IEntityTypeConfiguration<ReportReasonType>
    {
        public void Configure(EntityTypeBuilder<ReportReasonType> builder)
        {
            builder.HasData(
                new ReportReasonType
                {
                    Id = 1,
                    Name = "Спам"
                },
                new ReportReasonType { Id = 2, Name = "Ненормативная лексика" },
                new ReportReasonType { Id=3, Name="Нарушение авторских прав"},
                new ReportReasonType { Id=4, Name="Терроризм и экстремизм"},
                new ReportReasonType { Id=5, Name="Иное"});
        }
    }
}
