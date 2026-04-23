using Chronolibris.Application.Interfaces;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Options;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.DataAccess.Persistance.Repositories;
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

    public static class DependencyInjection
    {
        public static IServiceCollection AddExceptionMapper(this IServiceCollection services)
        {
            services.AddScoped<IExceptionMapper, ExceptionMapper>();
            return services;
        }
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
                //.UseExceptionProcessor();

                options.LogTo(Console.WriteLine, LogLevel.Error);

                options.EnableSensitiveDataLogging();

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
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IThemeRepository, ThemeRepository>();
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IBookFileRepository, BookFileRepository>();
            services.AddScoped<ITagsRepository, TagsRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IModerationTasksRepository, ModerationTasksRepository>();
            services.AddScoped<ISearchRepository, SearchRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddFileServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<BookStorageOptions>(configuration.GetSection("BookStorageOptions"));
            var minioOpts = configuration.GetSection("MinioOptions").Get<MinioOptions>()!;

            services.AddScoped<IMinioClient>(_ =>
                new MinioClient()
                    .WithEndpoint(minioOpts.Endpoint)
                    .WithCredentials(minioOpts.AccessKey, minioOpts.SecretKey)
                    .WithSSL(minioOpts.UseSSL)
                    //.WithHttpClient(new HttpClient(new HttpClientHandler
                    //{
                    //    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                    //}))
                    .Build());
            services.AddScoped<IStorageService, StorageService>();
            return services;
        }

        public static IServiceCollection AddIdentityRealization(this IServiceCollection services, 
            IConfiguration configuration)
        {

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddIdentity<User, IdentityRole<long>>(
                options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireUppercase = true;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";//@.-+
                }
                )
                .AddErrorDescriber<RussianIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
                //.AddDefaultTokenProviders();
            return services;
        }

        public static IServiceCollection AddFb2Converter(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFb2Converter, Fb2ConverterXReader>();
            return services;
        }

    }
}
