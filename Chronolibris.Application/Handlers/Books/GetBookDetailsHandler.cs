using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Interfaces;
using Chronolibris.Domain.Interfaces;
using MediatR;
using Chronolibris.Domain.Models;
using Chronolibris.Application.Requests.Books;

namespace Chronolibris.Application.Handlers.Books
{
    public class GetBookDetailsHandler : IRequestHandler<GetBookMetadataQuery, BookDetails?>
    {
        private readonly IUnitOfWork unitOfWork;
        public GetBookDetailsHandler(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        public async Task<BookDetails?> Handle(GetBookMetadataQuery request, CancellationToken cancellationToken)
        {
            return await unitOfWork.Books.GetBookWithRelationsAsync(request.BookId, request.UserId, request.Mode, cancellationToken);
        }
    }
}
