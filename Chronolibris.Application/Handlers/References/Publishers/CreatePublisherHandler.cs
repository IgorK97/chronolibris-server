using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Publishers
{
    public class CreatePublisherHandler : IRequestHandler<CreatePublisherCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePublisherHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreatePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = new Publisher
            {
                Id = 0,
                Name = request.Name.Trim(),
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Publishers.AddAsync(publisher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return publisher.Id;
        }
    }
}
