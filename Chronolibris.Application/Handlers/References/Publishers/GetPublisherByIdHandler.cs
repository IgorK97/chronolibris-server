using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Publishers
{
    public class GetPublisherByIdHandler : IRequestHandler<GetPublisherByIdQuery, PublisherDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPublisherByIdHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PublisherDto?> Handle(GetPublisherByIdQuery request, CancellationToken cancellationToken)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(request.Id, cancellationToken);
            if (publisher == null) return null;


            return new PublisherDto
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Description = publisher.Description,
                CreatedAt = publisher.CreatedAt,
            };
        }
    }
}
