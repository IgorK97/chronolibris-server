using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chronolibris.Infrastructure.DatabaseChecker
{

    /// <summary>
    /// Статический класс, предоставляющий методы для проверки и инициализации базы данных
    /// </summary>
    public static class DatabaseChecker
    {

        /// <summary>
        /// Асинхронно проверяет состояние базы данных, применяет ожидающие миграции 
        /// (если они есть) и гарантирует, что схема базы данных соответствует модели приложения
        /// </summary>
        public static async Task CheckDatabase(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            //Создание новой области видимости (scope) для корректного разрешения сервисов
            using var scope = serviceProvider.CreateScope();

            //Получение экземпляра контекста базы данных ApplicationDbContext
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //await context.Database.EnsureCreatedAsync();

            //Асинхронное применение всех ожидающих миграций к базе данных
            await context.Database.MigrateAsync();
        }
    }
}
