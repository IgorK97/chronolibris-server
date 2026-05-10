using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Publishers
{
    public class GetAllPublishersHandler : IRequestHandler<GetAllPublishersQuery, IEnumerable<PublisherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPublishersHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PublisherDto>> Handle(GetAllPublishersQuery request, CancellationToken cancellationToken)
        {
            var publishers = await _unitOfWork.Publishers.GetAllAsync(cancellationToken);

            return publishers.OrderBy(p => p.Name).Select(p => new PublisherDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
            });
        }
    }
}
