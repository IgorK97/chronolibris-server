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
    public class ContentParticipationConfiguration : IEntityTypeConfiguration<ContentParticipation>
    {
        public void Configure(EntityTypeBuilder<ContentParticipation> builder)
        {
            builder.ToTable("content_participations");

            builder.HasKey(cp => cp.Id);

            builder.HasOne(cp => cp.Person)
                .WithMany(p => p.ContentParticipations)
                .HasForeignKey(cp => cp.PersonId);

            builder.HasOne(cp => cp.PersonRole)
                .WithMany()
                .HasForeignKey(cp => cp.PersonRoleId);

            builder.HasOne(cp => cp.Content)
                .WithMany(c => c.Participations)
                .HasForeignKey(cp => cp.ContentId);

            builder.HasData(
                new ContentParticipation
                {
                    Id = 1,
                    ContentId = 1, // "Буддизм в Японии"
                    PersonId = 1,  // Татьяна Петровна Григорьева
                    PersonRoleId = 1, // Автор
                },
                new ContentParticipation
                {
                    Id = 2,
                    ContentId = 2, // "Структуры повседневности..."
                    PersonId = 2,  // Фернан Поль Ахилл Бродель
                    PersonRoleId = 1, // Автор
                }
            );
        }
    }
}
