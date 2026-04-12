using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Interfaces
{
    public interface IBookConversionService
    {
        Task ProcessAsync(long bookFileId);
    }
}
