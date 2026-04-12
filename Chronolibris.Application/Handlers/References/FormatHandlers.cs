using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.References
{
    public class GetAllFormatsHandler : IRequestHandler<GetAllFormatsQuery, IEnumerable<FormatDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllFormatsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FormatDto>> Handle(GetAllFormatsQuery request, CancellationToken cancellationToken)
        {
            var formats = await _unitOfWork.Formats.GetAllAsync(cancellationToken);
            return formats.Select(f => new FormatDto
            {
                Id = f.Id,
                Name = f.Name
            });
        }
    }

    public class GetFormatByIdHandler : IRequestHandler<GetFormatByIdQuery, FormatDto?>
    {
        private readonly IGenericRepository<Format> _repository;

        public GetFormatByIdHandler(IGenericRepository<Format> repository)
        {
            _repository = repository;
        }

        public async Task<FormatDto?> Handle(GetFormatByIdQuery request, CancellationToken cancellationToken)
        {
            var format = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (format == null) return null;

            return new FormatDto
            {
                Id = format.Id,
                Name = format.Name
            };
        }
    }

    public class CreateFormatHandler : IRequestHandler<CreateFormatCommand, int>
    {
        private readonly IGenericRepository<Format> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateFormatHandler(IGenericRepository<Format> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreateFormatCommand request, CancellationToken cancellationToken)
        {
            var format = new Format
            {
                Id=0,
                Name = request.Name
            };

            await _repository.AddAsync(format, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return format.Id;
        }
    }

    public class UpdateFormatHandler : IRequestHandler<UpdateFormatCommand, bool>
    {
        private readonly IGenericRepository<Format> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFormatHandler(IGenericRepository<Format> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateFormatCommand request, CancellationToken cancellationToken)
        {
            var format = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (format == null) return false;

            format.Name = request.Name;

            _repository.Update(format);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class DeleteFormatHandler : IRequestHandler<DeleteFormatCommand, bool>
    {
        private readonly IGenericRepository<Format> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFormatHandler(IGenericRepository<Format> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteFormatCommand request, CancellationToken cancellationToken)
        {
            var format = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (format == null) return false;

            _repository.Delete(format);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}