// File: Chronolibris.Application.Handlers.PublisherHandlers.cs
using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chronolibris.Application.Handlers
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
            var countries = await _unitOfWork.Countries.GetAllAsync(cancellationToken);

            return publishers.OrderBy(p=>p.Name).Select(p => new PublisherDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                CountryId = p.CountryId,
                CountryName = countries.FirstOrDefault(c => c.Id == p.CountryId)?.Name
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

            var country = await _unitOfWork.Countries.GetByIdAsync(publisher.CountryId, cancellationToken);

            return new PublisherDto
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Description = publisher.Description,
                CreatedAt = publisher.CreatedAt,
                CountryId = publisher.CountryId,
                CountryName = country?.Name
            };
        }
    }

    public class CreatePublisherHandler : IRequestHandler<CreatePublisherCommand, long>
    {
        private readonly IGenericRepository<Publisher> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePublisherHandler(IGenericRepository<Publisher> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreatePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = new Publisher
            {
                Id=0,
                Name = request.Name,
                Description = request.Description,
                CountryId = request.CountryId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repository.AddAsync(publisher, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return publisher.Id;
        }
    }

    public class UpdatePublisherHandler : IRequestHandler<UpdatePublisherCommand, bool>
    {
        private readonly IGenericRepository<Publisher> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePublisherHandler(IGenericRepository<Publisher> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (publisher == null) return false;

            publisher.Name = request.Name;
            publisher.Description = request.Description;
            publisher.CountryId = request.CountryId;

            _repository.Update(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class DeletePublisherHandler : IRequestHandler<DeletePublisherCommand, bool>
    {
        private readonly IGenericRepository<Publisher> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePublisherHandler(IGenericRepository<Publisher> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (publisher == null) return false;

            _repository.Delete(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}