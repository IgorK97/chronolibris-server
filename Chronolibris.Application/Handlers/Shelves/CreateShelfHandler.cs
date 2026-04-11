using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Shelves;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Shelves
{
    public class CreateShelfHandler : IRequestHandler<CreateShelfCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateShelfHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateShelfCommand request, CancellationToken cancellationToken)
        {
            

            var newShelf = new Shelf
            {
                CreatedAt = DateTime.UtcNow,
                Name = request.Name,
                UserId = request.UserId,
                ShelfTypeId = 3,
                Id = 0,
            };
            await _unitOfWork.Shelves.AddAsync(newShelf, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newShelf.Id;
        }
    }
}
