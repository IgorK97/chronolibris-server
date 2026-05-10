using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.References
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

            return publishers.OrderBy(p=>p.Name).Select(p => new PublisherDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
            });
        }
    }

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
                Id=0,
                Name = request.Name.Trim(),
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.Publishers.AddAsync(publisher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return publisher.Id;
        }
    }

    public class UpdatePublisherHandler : IRequestHandler<UpdatePublisherCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePublisherHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(request.Id, cancellationToken);
            if (publisher == null) return false;

            publisher.Name = request.Name.Trim();
            publisher.Description = request.Description;

            _unitOfWork.Publishers.Update(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class DeletePublisherHandler : IRequestHandler<DeletePublisherCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePublisherHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(request.Id, cancellationToken);
            if (publisher == null) return false;

            _unitOfWork.Publishers.Delete(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}