using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces.Repository
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        //Task<List<FtsConfigurationDto>> GetFtsConfigurationLanguages(CancellationToken token);
    }
}
