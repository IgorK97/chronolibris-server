using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Infrastructure.Services.Fb2Converter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace Chronolibris.Infrastructure.DataAccess.DependencyInjection
{
    public static class Fb2ConverterExtensions
    {
        public static IServiceCollection AddFb2Converter(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IFb2Converter, Fb2ConverterXReader>();

            return services;
        }
    }

}
