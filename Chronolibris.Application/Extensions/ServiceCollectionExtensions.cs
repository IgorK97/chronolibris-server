using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Chronolibris.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавляет MediatR и регистрирует все Handlers
        /// из Application-сборки
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        /// <returns>Текущая коллекция сервисов</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // ссылка на текущую сборку (Application-слой)
            Assembly applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

            // Регистрация MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);
            });

            return services;
        }
    }
}
