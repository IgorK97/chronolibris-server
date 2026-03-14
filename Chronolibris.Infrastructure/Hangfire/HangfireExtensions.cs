using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;

namespace Chronolibris.Infrastructure.DataAccess.Hangfire
{
    // Infrastructure/Extensions/HangfireExtensions.cs
    public static class HangfireExtensions
    {
        public static IServiceCollection AddHangfireInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c =>
                    c.UseNpgsqlConnection(
                        configuration.GetConnectionString("HangfireConnection")))
                .WithJobExpirationTimeout(TimeSpan.FromDays(7))
            );

            services.AddHangfireServer();

            return services;
        }
    }
}
