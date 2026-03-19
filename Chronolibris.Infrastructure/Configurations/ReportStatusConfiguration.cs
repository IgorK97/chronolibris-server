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
    public class ReportStatusConfiguration : IEntityTypeConfiguration<ReportStatus>
    {
        public void Configure(EntityTypeBuilder<ReportStatus> builder)
        {
            builder.HasData(
                new ReportStatus { Id = 1, Name = "Новое" },
                new ReportStatus { Id = 2, Name = "В работе" },
                new ReportStatus { Id = 3, Name = "Принято" },
                new ReportStatus { Id = 4, Name = "Отклонено" }
            );
        }
    }

}
