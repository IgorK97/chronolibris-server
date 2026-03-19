using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Configurations
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Report> builder)
        {
            builder.HasOne(r => r.ModerationTask)
                .WithMany(s=>s.Reports)
                .HasForeignKey(r => r.ModerationTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReasonType)
                .WithMany(rt => rt.Reports)
                .HasForeignKey(r => r.ReasonTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.TargetType)
                .WithMany(rt => rt.Reports)
                .HasForeignKey(r => r.TargetTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            
        }
    }
}
