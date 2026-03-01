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
    public class BookParticipationConfiguration : IEntityTypeConfiguration<BookParticipation>
    {
        public void Configure(EntityTypeBuilder<BookParticipation> builder)
        {
            builder.ToTable("book_participations");

            builder.HasKey(bp => bp.Id);

            builder.HasOne(bp => bp.Person)
                .WithMany(p => p.BookParticipations)
                .HasForeignKey(bp => bp.PersonId);

            builder.HasOne(bp => bp.PersonRole)
                .WithMany()
                .HasForeignKey(bp => bp.PersonRoleId);

            builder.HasOne(bp => bp.Book)
                .WithMany(b => b.Participations)
                .HasForeignKey(bp => bp.BookId);
        }
    }
}
