using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    public class SeekBookInShelvesHandler : IRequestHandler<SeekBookInShelvesQuery, long[]>
    {
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SeelBookInShelvesHandler"/>.
        /// </summary>
        /// <param name="unitOfWork">Интерфейс Unit of Work для взаимодействия с базой данных.</param>
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
