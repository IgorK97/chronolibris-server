using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.DataAccess.BackgroundServices;
using Chronolibris.Infrastructure.DataAccess.Persistance.Repositories;
using Chronolibris.Infrastructure.Files;
using Chronolibris.Infrastructure.Identity;
using Chronolibris.Infrastructure.Persistance;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chronolibris.Infrastructure.DependencyInjection
{

    /// <summary>
    /// Статический класс для регистрации всех зависимостей инфраструктурного уровня 
    /// в контейнере служб <see cref="IServiceCollection"/>.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Регистрирует контекст базы данных PostgreSQL и все репозитории приложения.
        /// </summary>
        /// <param name="services">Коллекция служб для расширения.</param>
        /// <param name="configuration">Конфигурация приложения для получения строки подключения.</param>
        /// <returns>Обновленная коллекция служб.</returns>
        public static IServiceCollection AddDatabaseInfrastructure(this IServiceCollection services, 
            IConfiguration configuration)
        {

            // Регистрация DbContext для PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

                // Настройка для автоматического преобразования имен свойств в snake_case в БД
                options.UseSnakeCaseNamingConvention();
            });

            // Регистрация обобщенных репозиториев (Scoped lifetime)
            services.AddScoped<IGenericRepository<Content>, GenericRepository<Content>>();
            services.AddScoped<IGenericRepository<Person>, GenericRepository<Person>>();
            services.AddScoped<IGenericRepository<Publisher>, GenericRepository<Publisher>>();
            services.AddScoped<IGenericRepository<PersonRole>, GenericRepository<PersonRole>>();
            services.AddScoped<IGenericRepository<Shelf>, GenericRepository<Shelf>>();
            services.AddScoped<IGenericRepository<Comment>, GenericRepository<Comment>>();
            services.AddScoped<IGenericRepository<Language>, GenericRepository<Language>>();

            // Регистрация специфических репозиториев (Scoped lifetime)
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookmarkRepository, BookmarkRepository>();
            services.AddScoped<IReviewReactionsRepository,  ReviewReactionsRepository>();
            services.AddScoped<ICommentReactionsRepository, CommentReactionsRepository>();
            services.AddScoped<IReviewRepository,  ReviewRepository>();
            services.AddScoped<ISelectionsRepository, SelectionsRepository>();
            services.AddScoped<IShelfRepository, ShelvesRepository>();
            services.AddScoped<IReadingProgressRepository, ReadingProgressRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();

            // Регистрация Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        /// <summary>
        /// Регистрирует службы, связанные с системой аутентификации и идентификации (ASP.NET Core Identity).
        /// </summary>
        /// <param name="services">Коллекция служб для расширения.</param>
        /// <param name="configuration">Конфигурация приложения (не используется явно в теле метода).</param>
        /// <returns>Обновленная коллекция служб.</returns>
        public static IServiceCollection AddIdentityRealization(this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Регистрация кастомного сервиса идентификации
            services.AddScoped<IIdentityService, IdentityService>();

            // Регистрация стандартной реализации Identity
            services.AddIdentity<User, IdentityRole<long>>(
                //options =>
                //{
                //    options.Password.RequiredLength = 8;
                //    options.Password.RequireUppercase = true;
                //    options.User.RequireUniqueEmail = true;
                //}
                )
                // Указывает, что Identity будет использовать ApplicationDbContext
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddHostedService<TokenCleanupService>();

            return services;
        }

        /// <summary>
        /// Регистрирует провайдер для доступа к файлам книг.
        /// </summary>
        /// <param name="services">Коллекция служб для расширения.</param>
        /// <param name="configuration">Конфигурация приложения для получения пути к папке с книгами.</param>
        /// <returns>Обновленная коллекция служб.</returns>
        public static IServiceCollection AddFileProviderInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            var booksFolder = configuration["BooksFolder"] ?? throw new InvalidOperationException("BooksFolder not configured.");
            services.AddSingleton<IBookFileProvider>(new BookFileProvider(booksFolder));

            return services;
        }

    }
}
