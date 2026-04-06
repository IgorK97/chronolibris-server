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
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            DateTime dt = new DateTime(2025, 11, 20, 0, 0, 0, DateTimeKind.Utc);


            builder.HasData(
                new Person
                {
                    Id = 1,
                    CreatedAt = dt,
                    Name = "Татьяна Петровна Григорьева",
                    //ImagePath = "none",
                    Description = "Советский и российский востоковед-японист, литературовед, переводчица...",
                },
                new Person
                {
                    Id = 2,
                    CreatedAt = dt,
                    Name = "Фернан Поль Ахилл Бродель",
                    //ImagePath = "Brodel/MainFile.jpeg",
                    Description = "Французский историк, член Французской академии...",
                }
            );
        }
    }
}
