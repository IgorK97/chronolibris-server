using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Shelves
{
    public class SeekBookInShelvesHandler : IRequestHandler<SeekBookInShelvesQuery, long[]>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SeekBookInShelvesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long[]> Handle(SeekBookInShelvesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Shelves.SeekBookInShelves(request.UserId, request.BookId);


        }
    }
}
