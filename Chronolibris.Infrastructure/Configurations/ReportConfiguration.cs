using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
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

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(r=>r.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            //builder.ToTable(r => {
            //    r.HasCheckConstraint("ck_report_description_min_length",
            //        "LENGTH(TRIM(description))>=20");
            //});


        }
    }
}
