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
    public class ModerationTaskConfiguration : IEntityTypeConfiguration<ModerationTask>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ModerationTask> builder)
        {
            builder.HasOne(mt=>mt.Status)
                .WithMany(s=>s.Tasks)
                .HasForeignKey(mt=>mt.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mt => mt.ReasonType)
                .WithMany(s => s.ModerationTasks)
                .HasForeignKey(mt => mt.ReasonTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mt=>mt.TargetType)
                .WithMany(rtt=>rtt.ModerationTasks)
                .HasForeignKey(mt=>mt.TargetTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(mt => mt.ModeratedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
