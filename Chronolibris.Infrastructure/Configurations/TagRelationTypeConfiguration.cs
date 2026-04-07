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
    public class TagRelationTypeConfiguration : IEntityTypeConfiguration<TagRelationType>
    {
        public void Configure(EntityTypeBuilder<TagRelationType> builder)
        {


            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(200);

            builder.HasIndex(t => t.Name)
                .IsUnique();

            builder.HasData(
                new TagRelationType { Id = 1, Name = "Синоним", Description = "Синонимия (теги обозначают одно и то же понятие)" },
                new TagRelationType { Id = 2, Name = "Часть", Description = "Часть/целое" }

            );
        }
    }

}
