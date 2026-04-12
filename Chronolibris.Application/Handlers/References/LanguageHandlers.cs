using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Models;
using Chronolibris.Application.Requests.References;

namespace Chronolibris.Application.Handlers.References
{
    public class GetAllLanguagesHandler : IRequestHandler<GetAllLanguagesQuery, IEnumerable<LanguageDto>>
    {
        private readonly IGenericRepository<Language> _repository;

        public GetAllLanguagesHandler(IGenericRepository<Language> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LanguageDto>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
        {
            var languages = await _repository.GetAllAsync(cancellationToken);
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
        private readonly IGenericRepository<Language> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateLanguageHandler(IGenericRepository<Language> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = new Language
            {
                Id=0,
                Name = request.Name,
            };

            await _repository.AddAsync(language, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return language.Id;
        }
    }

    public class GetFtsConfigurationsHandler : IRequestHandler<GetFtsConfigurationsQuery, IEnumerable<FtsConfigurationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFtsConfigurationsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FtsConfigurationDto>> Handle(GetFtsConfigurationsQuery request, CancellationToken cancellationToken)
        {


            var configurations = await _unitOfWork.Languages.GetFtsConfigurationLanguages(cancellationToken);

            return configurations;
        }
    }

    public class UpdateLanguageHandler : IRequestHandler<UpdateLanguageCommand, bool>
    {
        private readonly IGenericRepository<Language> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLanguageHandler(IGenericRepository<Language> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (language == null) return false;

            language.Name = request.Name;

            _repository.Update(language);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class DeleteLanguageHandler : IRequestHandler<DeleteLanguageCommand, bool>
    {
        private readonly IGenericRepository<Language> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLanguageHandler(IGenericRepository<Language> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = await _repository.GetByIdAsync(request.id, cancellationToken);
            if (language == null) return false;

            _repository.Delete(language);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
