using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Fb2Converter.Interfaces;
using Chronolibris.Infrastructure.DataAccess.Fb2Converter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace Chronolibris.Infrastructure.DataAccess.DependencyInjection
{
    public static class Fb2ConverterExtensions
    {
        /// <summary>
        /// Добавляет к уже зарегистрированному IMinioClient сервисы конвертера книг:
        ///   • BookStorageOptions  (из секции "BookStorageOptions")
        ///   • IBookStorage        → MinioBookStorage  (Singleton, использует общий IMinioClient)
        ///   • IFb2Converter       → Fb2ConverterService (Scoped)
        ///
        /// Предполагается, что IMinioClient уже зарегистрирован — как в вашем AddFileServices.
        /// Вызывайте этот метод ПОСЛЕ AddFileServices.
        /// </summary>
        public static IServiceCollection AddFb2Converter(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. Привязываем BookStorageOptions из конфига
            services.Configure<BookStorageOptions>(
                configuration.GetSection(BookStorageOptions.SectionName));

            // 2. MinioBookStorage как Singleton:
            //    - IMinioClient берётся из DI (уже зарегистрирован в AddFileServices)
            //    - bucket/prefix берутся из BookStorageOptions
            //    Singleton безопасен: IMinioClient thread-safe, своего состояния нет
            services.AddSingleton<IBookStorage>(sp =>
            {
                var minioClient = sp.GetRequiredService<IMinioClient>();
                var opts = sp.GetRequiredService<IOptions<BookStorageOptions>>().Value;
                return new MinioBookStorage(minioClient, opts.BucketName, opts.Prefix);
            });

            // 3. Конвертер — Scoped (на один HTTP-запрос / единицу работы)
            services.AddScoped<IFb2Converter, Fb2ConverterService>();

            return services;
        }

        /// <summary>
        /// Вариант для тестов и разработки без реального MinIO.
        /// Регистрирует InMemoryBookStorage вместо MinioBookStorage.
        /// IMinioClient НЕ нужен.
        /// </summary>
        public static IServiceCollection AddFb2ConverterInMemory(
            this IServiceCollection services)
        {
            services.AddSingleton<IBookStorage, InMemoryBookStorage>();
            services.AddScoped<IFb2Converter, Fb2ConverterService>();
            return services;
        }
    }

}
