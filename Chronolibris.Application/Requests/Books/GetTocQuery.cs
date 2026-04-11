using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetTocQuery(long BookFileId) : IRequest<string?>;
}
