using Chronolibris.Application.Interfaces;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.DataAccess.BackgroundServices;
using Chronolibris.Infrastructure.DataAccess.Files;
using Chronolibris.Infrastructure.DataAccess.Persistance;
using Chronolibris.Infrastructure.DataAccess.Persistance.Repositories;
using Chronolibris.Infrastructure.Files;
using Chronolibris.Infrastructure.Identity;
using Chronolibris.Infrastructure.Persistance;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Chronolibris.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

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
            services.AddScoped<IGenericRepository<Country>, GenericRepository<Country>>();
            services.AddScoped<IGenericRepository<Format>, GenericRepository<Format>>();
            services.AddScoped<IGenericRepository<Series>, GenericRepository<Series>>();
            services.AddScoped<IGenericRepository<BookFile>, GenericRepository<BookFile>>();


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
            services.AddScoped<IThemeRepository, ThemeRepository>();
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IBookFileRepository, BookFileRepository>();

            // Регистрация Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddFileServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Привязываем настройки из JSON к классу MinioOptions
            var minioOptions = configuration.GetSection("MinioOptions").Get<MinioOptions>();
            services.Configure<MinioOptions>(configuration.GetSection("MinioOptions"));

            // 2. Регистрируем IMinioClient как Singleton или Scoped
            services.AddScoped<IMinioClient>(sp =>
            {
                var client = new MinioClient()
                    .WithEndpoint(minioOptions!.Endpoint)
                    .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);

                if (minioOptions.UseSSL) client.WithSSL();

                return client.Build();
            });

            // 3. Регистрируем твой сервис для работы с файлами
            services.AddScoped<IFileService, MinioFileService>();

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
