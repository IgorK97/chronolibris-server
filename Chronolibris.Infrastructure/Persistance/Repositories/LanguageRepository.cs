using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using Chronolibris.Infrastructure.Data;
using Chronolibris.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chronolibris.Infrastructure.DataAccess.Persistance.Repositories
{
    internal class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {


       
        public LanguageRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<List<FtsConfigurationDto>> GetFtsConfigurationLanguages(CancellationToken token)
        {
            var sql = @"
                    SELECT 
                        oid::bigint AS config_oid,
                        cfgname AS config_name
                    FROM 
                        pg_ts_config
                    ORDER BY 
                        cfgname";

            var configurations = await _context.Database
                .SqlQueryRaw<FtsConfigurationDto>(sql)
                .ToListAsync(token);

            return configurations;
        }

    }
}
