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
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasOne(t => t.ParentTag)
                .WithMany(t => t.ChildTags)
                .HasForeignKey(t => t.ParentTagId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(t => t.RelationType)
                .WithMany(t => t.Tags)
                .HasForeignKey(t => t.RelationTypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_tags_name_not_empty",
                "LENGTH(TRIM(name))>0"));

        }
    }
}
