using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class PersonRoleConfiguration : IEntityTypeConfiguration<PersonRole>
    {
        public void Configure(EntityTypeBuilder<PersonRole> builder)
        {
            builder.Property(e => e.Kind)
                .HasColumnType("person_role_kind")
                .HasDefaultValue(PersonRoleKind.Both);

            builder.HasData(
                new PersonRole { Id = 1, Name = "Автор", Kind=PersonRoleKind.Content },
                new PersonRole { Id = 2, Name = "Переводчик", Kind=PersonRoleKind.Book },
                new PersonRole { Id = 3, Name = "Редактор", Kind=PersonRoleKind.Book },
                new PersonRole { Id = 4, Name = "Иллюстратор", Kind=PersonRoleKind.Both },
                new PersonRole { Id = 5, Name = "Составитель", Kind=PersonRoleKind.Book },
                new PersonRole { Id = 6, Name = "Корректор", Kind=PersonRoleKind.Book  },
                new PersonRole { Id = 7, Name = "Научный редактор", Kind=PersonRoleKind.Book  },
                new PersonRole { Id = 8, Name = "Редактор перевода", Kind=PersonRoleKind.Book  },
                new PersonRole { Id = 9, Name = "Адресат", Kind=PersonRoleKind.Content}
            );
        }
    }
}
