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
                new PersonRole { Id = 1, Name = "Автор" },
                new PersonRole { Id = 2, Name = "Переводчик" },
                new PersonRole { Id = 3, Name = "Редактор" },
                new PersonRole { Id = 4, Name = "Иллюстратор" },
                new PersonRole { Id = 5, Name = "Составитель" },
                new PersonRole { Id = 6, Name = "Корректор"  },
                new PersonRole { Id = 7, Name = "Научный редактор"  },
                //new PersonRole { Id = 8, Name = "Литературный редактор"  },
                //new PersonRole { Id = 9, Name = "Технический редактор" },
                new PersonRole { Id = 8, Name = "Редактор перевода"  },
                new PersonRole { Id = 9, Name = "Комментатор" },
                new PersonRole { Id = 10, Name = "Адресат"}

                //new PersonRole { Id = 11, Name = "Оцифровщик" },
                //new PersonRole { Id = 12, Name = "Автор предисловия" },
                //new PersonRole { Id = 13, Name = "Автор послесловия" },
                //new PersonRole { Id = 15, Name = "Дизайнер" }
            );
        }
    }
}
