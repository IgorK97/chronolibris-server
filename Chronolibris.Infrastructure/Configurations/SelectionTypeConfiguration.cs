//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chronolibris.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace Chronolibris.Infrastructure.DataAccess.Configurations
//{
//    public class SelectionTypeConfiguration :IEntityTypeConfiguration<SelectionType>
//    {
//        public void Configure(EntityTypeBuilder<SelectionType> builder)
//        {
//            builder.HasData(
//                new SelectionType
//                {
//                    Id = 1,
//                    Name = "Newest"
//                },
//                new SelectionType
//                {
//                    Id = 2,
//                    Name = "Popular"
//                },
//                new SelectionType
//                {
//                    Id = 3,
//                    Name = "Manual"
//                });
//        }
//    }
//}
