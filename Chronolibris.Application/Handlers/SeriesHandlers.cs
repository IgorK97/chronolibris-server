// File: Chronolibris.Application.Handlers.SeriesHandlers.cs
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
    public class GetAllSeriesHandler : IRequestHandler<GetAllSeriesQuery, IEnumerable<SeriesDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSeriesHandler(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SeriesDto>> Handle(GetAllSeriesQuery request, CancellationToken cancellationToken)
        {
            var series = await _unitOfWork.Series.GetAllAsync(cancellationToken);
            var publishers = await _unitOfWork.Publishers.GetAllAsync(cancellationToken);
            var books = await _unitOfWork.Books.GetAllAsync(cancellationToken);

            return series.Select(s => new SeriesDto
            {
                Id = s.Id,
                Name = s.Name,
                PublisherId = s.PublisherId,
                PublisherName = publishers.FirstOrDefault(p => p.Id == s.PublisherId)?.Name,
                CreatedAt = s.CreatedAt,
                BooksCount = books.Count(b => /* логика подсчета книг в серии */ true)
            });
        }
    }

    public class GetSeriesByIdHandler : IRequestHandler<GetSeriesByIdQuery, SeriesDto?>
    {
        private readonly IGenericRepository<Series> _repository;
        private readonly IGenericRepository<Publisher> _publisherRepository;

        public GetSeriesByIdHandler(
            IGenericRepository<Series> repository,
            IGenericRepository<Publisher> publisherRepository)
        {
            _repository = repository;
            _publisherRepository = publisherRepository;
        }

        public async Task<SeriesDto?> Handle(GetSeriesByIdQuery request, CancellationToken cancellationToken)
        {
            var series = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (series == null) return null;

            var publisher = await _publisherRepository.GetByIdAsync(series.PublisherId, cancellationToken);

            return new SeriesDto
            {
                Id = series.Id,
                Name = series.Name,
                PublisherId = series.PublisherId,
                PublisherName = publisher?.Name,
                CreatedAt = series.CreatedAt,
                BooksCount = 0 // Можно доработать подсчет
            };
        }
    }

    public class CreateSeriesHandler : IRequestHandler<CreateSeriesCommand, long>
    {
        private readonly IGenericRepository<Series> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSeriesHandler(IGenericRepository<Series> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateSeriesCommand request, CancellationToken cancellationToken)
        {
            var series = new Series
            {
                Id=0,
                Name = request.Name,
                PublisherId = request.PublisherId,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(series, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return series.Id;
        }
    }

    public class UpdateSeriesHandler : IRequestHandler<UpdateSeriesCommand, bool>
    {
        private readonly IGenericRepository<Series> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSeriesHandler(IGenericRepository<Series> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateSeriesCommand request, CancellationToken cancellationToken)
        {
            var series = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (series == null) return false;

            series.Name = request.Name;
            series.PublisherId = request.PublisherId;

            _repository.Update(series);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class DeleteSeriesHandler : IRequestHandler<DeleteSeriesCommand, bool>
    {
        private readonly IGenericRepository<Series> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSeriesHandler(IGenericRepository<Series> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteSeriesCommand request, CancellationToken cancellationToken)
        {
            var series = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (series == null) return false;

            _repository.Delete(series);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}