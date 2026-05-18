using Chronolibris.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chronolibris.Infrastructure.DatabaseChecker
{
    public static class DatabaseChecker
    {
        /// <summary>
        /// Асинхронно проверяет состояние базы данных, применяет ожидающие миграции 
        /// (если они есть) и гарантирует, что схема базы данных соответствует модели приложения
        /// </summary>
        public static async Task CheckDatabase(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //await context.Database.EnsureCreatedAsync();

            //Асинхронное применение всех ожидающих миграций к базе данных
            await context.Database.MigrateAsync();
        }
    }
}
