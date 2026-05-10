using Chronolibris.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chronolibris.Infrastructure.Configurations
{
    public class ThemeConfiguration : IEntityTypeConfiguration<Theme>
    {
        public void Configure(EntityTypeBuilder<Theme> builder)
        {
            builder.HasMany(th => th.SubThemes)
                .WithOne(th => th.ParentTheme)
                .HasForeignKey(th=>th.ParentThemeId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(t => t.HasCheckConstraint(
                "ck_themes_name_not_empty",
                "LENGTH(TRIM(name))>0"));

            builder.HasData(
                new Theme { Id = 1, Name = "Политическая история" },
                new Theme { Id = 2, Name = "Военная история" },
                new Theme { Id = 3, Name = "Философия" },
                new Theme { Id = 6, Name="Искусство"},
                new Theme { Id = 7, Name="Религия"},

                    new Theme { Id = 8, Name = "Всеобщая и мировая история", ParentThemeId = 1 },
                    new Theme { Id = 9, Name = "Региональная и национальная история", ParentThemeId = 1 },
                        new Theme { Id = 10, Name = "История Европы", ParentThemeId=9 },
                        new Theme { Id = 11, Name = "История России", ParentThemeId=9 },
                        new Theme { Id = 12, Name = "История Азии", ParentThemeId = 9 },
                            new Theme { Id = 5, Name = "История Японии", ParentThemeId = 12},
                            new Theme { Id = 4, Name = "История Китая", ParentThemeId = 12 },
                        new Theme { Id = 13, Name = "История Африки" , ParentThemeId = 9 },
                        new Theme { Id = 14, Name = "История Америки" , ParentThemeId = 9 },
                        new Theme { Id = 15, Name = "История Австралазии и Тихоокеании" , ParentThemeId = 9 },
                        new Theme { Id = 16, Name = "История других земель" , ParentThemeId = 9 },
                    new Theme { Id = 17, Name = "История: отдельные темы и события", ParentThemeId = 1 }, 

                    new Theme { Id = 18, Name = "Западная философия", ParentThemeId = 3 },
                        new Theme { Id = 19, Name = "Античная философия", ParentThemeId = 18 },
                        new Theme { Id = 20, Name = "Философия Средних веков и эпохи Ренессанса", ParentThemeId = 18 },
                        new Theme { Id = 21, Name = "Философия эпохи Нового времени", ParentThemeId = 18 },
                        new Theme { Id = 22, Name = "Современная философия", ParentThemeId = 18 },
                    new Theme { Id = 23, Name = "Восточная философия", ParentThemeId = 3 },
                    new Theme { Id = 24, Name = "Исламская и арабская философия", ParentThemeId = 3 },
                    new Theme { Id = 25, Name = "Социальная и политическая философия", ParentThemeId = 3 },
                    new Theme { Id = 26, Name = "Русская философия", ParentThemeId = 3}

            );
        }
    }
}
