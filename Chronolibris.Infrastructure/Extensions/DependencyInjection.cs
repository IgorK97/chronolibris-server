using Chronolibris.Application.Interfaces;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Options;
using Chronolibris.Infrastructure.Data;
//using Chronolibris.Infrastructure.DataAccess.BackgroundServices;
using Chronolibris.Infrastructure.DataAccess.Persistance;
using Chronolibris.Infrastructure.DataAccess.Persistance.Repositories;
//using Chronolibris.Infrastructure.Files;
using Chronolibris.Infrastructure.Persistance;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Chronolibris.Infrastructure.Persistence.Repositories;
using Chronolibris.Infrastructure.Services.Fb2Converter;
using Chronolibris.Infrastructure.Services.Files;
using Chronolibris.Infrastructure.Services.IdentityService;
using Chronolibris.Infrastructure.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minio;
using Npgsql;
using Npgsql.NameTranslation;

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
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration
                .GetConnectionString("DefaultConnection"));

            var translator = new NpgsqlSnakeCaseNameTranslator();

            dataSourceBuilder.MapEnum<ContentNature>("content_nature_enum", translator);
            dataSourceBuilder.MapEnum<PersonRoleKind>("person_role_kind", translator);

            var dataSource = dataSourceBuilder.Build();

            // Регистрация DbContext для PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(dataSource);

                // 1. Добавляем логирование в консоль
                options.LogTo(Console.WriteLine, LogLevel.Information);

                // 2. (Опционально) Чтобы видеть значения параметров (например, сам текст комментария),
                // а не просто @p0, добавь эту строку:
                options.EnableSensitiveDataLogging();

                // Настройка для автоматического преобразования имен свойств в snake_case в БД
                options.UseSnakeCaseNamingConvention();
            });

            //services.Configure<ReportingOptions>(
            //    configuration.GetSection(ReportingOptions.SectionName));

            services.AddSingleton<ReportingOptions>(sp =>
            {
                var opts = new ReportingOptions();
                configuration.GetSection(ReportingOptions.SectionName).Bind(opts);
                return opts;
            });


            // Регистрация обобщенных репозиториев
            services.AddScoped<IGenericRepository<Content>, GenericRepository<Content>>();
            services.AddScoped<IGenericRepository<Person>, GenericRepository<Person>>();
            services.AddScoped<IGenericRepository<Publisher>, GenericRepository<Publisher>>();
            services.AddScoped<IGenericRepository<PersonRole>, GenericRepository<PersonRole>>();
            services.AddScoped<IGenericRepository<Shelf>, GenericRepository<Shelf>>();
            services.AddScoped<IGenericRepository<Comment>, GenericRepository<Comment>>();
            services.AddScoped<IGenericRepository<Language>, GenericRepository<Language>>();
            services.AddScoped<IGenericRepository<Country>, GenericRepository<Country>>();
            services.AddScoped<IGenericRepository<Format>, GenericRepository<Format>>();
            services.AddScoped<IGenericRepository<BookFile>, GenericRepository<BookFile>>();
            services.AddScoped<IGenericRepository<Report>,  GenericRepository<Report>>();
            services.AddScoped<IGenericRepository<ModerationTask>, GenericRepository<ModerationTask>>();

            // Регистрация репозиториев
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookmarkRepository, BookmarkRepository>();
            services.AddScoped<IReviewReactionsRepository,  ReviewReactionsRepository>();
            services.AddScoped<ICommentReactionsRepository, CommentReactionsRepository>();
            services.AddScoped<IReviewRepository,  ReviewRepository>();
            services.AddScoped<ISelectionsRepository, SelectionsRepository>();
            services.AddScoped<IShelfRepository, ShelvesRepository>();
            services.AddScoped<IReadingProgressRepository, ReadingProgressRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IThemeRepository, ThemeRepository>();
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IBookFileRepository, BookFileRepository>();
            services.AddScoped<ITagsRepository, TagsRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IModerationTasksRepository, ModerationTasksRepository>();
            services.AddScoped<IBookSearchRepository, SearchRepository>();
            services.AddScoped<IReferenceSearchRepository, ReferenceSearchRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddFileServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Привязываем настройки из JSON к классу MinioOptions
            //var minioOptions = configuration.GetSection("MinioOptions").Get<MinioOptions>();
            //services.Configure<MinioOptions>(configuration.GetSection("MinioOptions"));

            services.Configure<BookStorageOptions>(
                configuration.GetSection("BookStorageOptions"));

            services.Configure<UploadStorageOptions>(
                configuration.GetSection("UploadStorageOptions"));

            var minioOpts = configuration
                .GetSection("MinioOptions")
                .Get<MinioOptions>()!;

            services.AddScoped<IMinioClient>(_ =>
                new MinioClient()
                    .WithEndpoint(minioOpts.Endpoint)
                    .WithCredentials(minioOpts.AccessKey, minioOpts.SecretKey)
                    .WithSSL(minioOpts.UseSSL)
                    .Build());

            //// 2. Регистрируем IMinioClient как Singleton или Scoped
            //services.AddScoped<IMinioClient>(sp =>
            //{
            //    var client = new MinioClient()
            //        .WithEndpoint(minioOptions!.Endpoint)
            //        .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey);

            //    if (minioOptions.UseSSL) client.WithSSL();

            //    return client.Build();
            //});

            // 3. Регистрируем твой сервис для работы с файлами
            //services.AddScoped<IFileService, MinioFileService>();
            //services.AddScoped<IMinioService, MinioService>();
            services.AddScoped<IStorageService, StorageService>();

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
                .AddEntityFrameworkStores<ApplicationDbContext>();
                //.AddDefaultTokenProviders();

            //services.AddHostedService<TokenCleanupService>();

            return services;
        }

        public static IServiceCollection AddJobs(this IServiceCollection services,
    IConfiguration configuration)
        {

            services.AddScoped<IBookConversionService, BookConversionService>();



            return services;
        }

        public static IServiceCollection AddFb2Converter(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            services.AddScoped<IFb2Converter, Fb2ConverterXReader>();

            return services;
        }

    }
}
