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
    internal class BookFileStatusConfiguration : IEntityTypeConfiguration<BookFileStatus>
    {
        public void Configure(EntityTypeBuilder<BookFileStatus> builder)
        {
            builder.HasData(new BookFileStatus { Id = 1, Name = "В ожидании загрузки" },
                new BookFileStatus { Id=2, Name="Файл загружен, не обработан"},
                new BookFileStatus { Id=3, Name="Обработка"},
                new BookFileStatus { Id=4, Name="Готов"},
                new BookFileStatus { Id=5, Name="Ошибка"});
        }
    }
}
