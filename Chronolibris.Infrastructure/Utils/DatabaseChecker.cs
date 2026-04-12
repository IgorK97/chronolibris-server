using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chronolibris.Infrastructure.DatabaseChecker
{

    /// <summary>
    /// Статический класс, предоставляющий методы для проверки и инициализации базы данных.
    /// Обычно вызывается при запуске приложения для обеспечения актуальности схемы БД.
    /// </summary>
    public static class DatabaseChecker
    {

        /// <summary>
        /// Асинхронно проверяет состояние базы данных, применяет ожидающие миграции 
        /// (если они есть) и гарантирует, что схема базы данных соответствует модели приложения.
        /// </summary>
        /// <param name="serviceProvider">Поставщик служб, используемый для получения контекста базы данных.</param>
        /// <param name="configuration">Конфигурация приложения (не используется явно в текущем коде, но является частью сигнатуры).</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        public static async Task CheckDatabase(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // Создание новой области видимости (scope) для корректного разрешения сервисов.
            using var scope = serviceProvider.CreateScope();

            // Получение экземпляра контекста базы данных ApplicationDbContext.
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //await context.Database.EnsureCreatedAsync();

            // Асинхронное применение всех ожидающих миграций к базе данных.
            await context.Database.MigrateAsync();
  

        }

        
    }
}
