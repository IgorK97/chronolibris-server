using MediatR;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.References
{
    public class GetAllLanguagesHandler : IRequestHandler<GetAllLanguagesQuery, IEnumerable<LanguageDto>>
    {
        private readonly ISearchRepository _repository;

        public GetAllLanguagesHandler(ISearchRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LanguageDto>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
        {
            var languages = await _repository.GetAllLanguagesAsync(cancellationToken);
            return languages.Select(l => new LanguageDto
            {
                Id = l.Id,
                Name = l.Name,
            });
        }
    }

    public class GetLanguageByIdHandler : IRequestHandler<GetLanguageByIdQuery, LanguageDto?>
    {
        private readonly IGenericRepository<Language> _repository;

        public GetLanguageByIdHandler(IGenericRepository<Language> repository)
        {
            _repository = repository;
        }

        public async Task<LanguageDto?> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
        {
            var language = await _repository.GetByIdAsync(request.id, cancellationToken);
            if (language == null) return null;

            return new LanguageDto
            {
                Id = language.Id,
                Name = language.Name,
            };
        }
    }

    public class CreateLanguageHandler : IRequestHandler<CreateLanguageCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLanguageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = new Language
            {
                Id=0,
                Name = request.Name.Trim(),
            };

            await _unitOfWork.Languages.AddAsync(language, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return language.Id;
        }
    }

    public class UpdateLanguageHandler : IRequestHandler<UpdateLanguageCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLanguageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = await _unitOfWork.Languages.GetByIdAsync(request.Id, cancellationToken);
            if (language == null) return false;

            language.Name = request.Name.Trim();

            _unitOfWork.Languages.Update(language);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class DeleteLanguageHandler : IRequestHandler<DeleteLanguageCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLanguageHandler(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = await _unitOfWork.Languages.GetByIdAsync(request.id, cancellationToken);
            if (language == null) return false;

            _unitOfWork.Languages.Delete(language);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
