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
    public record GetBookMetadataQuery(long bookId, long userId, bool mode) : IRequest<BookDetails>;
}
