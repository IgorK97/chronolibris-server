using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetBookMetadataQuery(long BookId, long UserId, bool Mode) : IRequest<BookDetails>;
}
