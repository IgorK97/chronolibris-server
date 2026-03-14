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
    public class ContentTypeConfiguration : IEntityTypeConfiguration<ContentType>
    {
        public void Configure(EntityTypeBuilder<ContentType> builder)
        {
            builder.Property(ct=>ct.Nature)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasData(
                 new ContentType { Id=1, Name="Дневник", Nature=ContentNature.Document },
                 new ContentType { Id=2, Name="Письмо", Nature=ContentNature.Document },
                 new ContentType { Id = 3, Name = "Мемуары", Nature = ContentNature.Document },
                 new ContentType { Id = 4, Name = "Автобиография", Nature = ContentNature.Document },
                 new ContentType { Id = 5, Name = "Хроника", Nature = ContentNature.Document },
                 new ContentType { Id = 6, Name = "Летопись", Nature = ContentNature.Document },
                 new ContentType { Id = 7, Name = "Манифест", Nature = ContentNature.Document },
                 new ContentType { Id = 8, Name = "Речь", Nature = ContentNature.Document },
                 new ContentType { Id = 9, Name = "Указ", Nature = ContentNature.Document },


                 new ContentType { Id = 10, Name = "Рассказ", Nature = ContentNature.Work },
                 new ContentType { Id = 11, Name = "Роман", Nature = ContentNature.Work },
                 new ContentType { Id = 12, Name = "Философский трактат", Nature = ContentNature.Work },
                 new ContentType { Id = 13, Name = "Богословский трактат", Nature = ContentNature.Work },
                 new ContentType { Id = 14, Name = "Политический трактат", Nature = ContentNature.Work },
                 new ContentType { Id = 15, Name = "Биография", Nature = ContentNature.Work },
                 new ContentType { Id = 16, Name = "Путевые заметки", Nature = ContentNature.Work },
                 new ContentType { Id = 17, Name = "Сборник", Nature = ContentNature.Work },
                 new ContentType { Id = 18, Name = "Учебник", Nature = ContentNature.Work },


                 new ContentType { Id = 19, Name = "Историческое исследование", Nature = ContentNature.Analysis },
                 new ContentType { Id = 20, Name = "Монография", Nature = ContentNature.Analysis },
                 new ContentType { Id = 21, Name = "Научная статья", Nature = ContentNature.Analysis },


                 new ContentType { Id = 22, Name = "Неизвестно", Nature = ContentNature.Unknown }
                 );
        }
    }
}
