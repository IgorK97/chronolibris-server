using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.Property(bm => bm.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            //builder.ToTable("bookmarks");

            //builder.HasOne(b => b.Book)
            //       .WithMany()
            //       .HasForeignKey(b => b.BookId);

            builder.HasOne<User>() 
                   .WithMany()    
                   .HasForeignKey(b => b.UserId) 
                   .HasPrincipalKey(u => u.Id);  
                                                
        }
    }
}
