using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests
{
    public record SeekBookInShelvesQuery(long UserId, long BookId) : IRequest<long[]>;
}
