using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Jobs
{
    public interface IBookConversionJob
    {
        Task ProcessAsync(long bookFileId);
    }
}
