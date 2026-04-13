using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Exceptions;

namespace Chronolibris.Application.Handlers.References
{
    public class GetAllThemesHandler : IRequestHandler<GetAllThemesQuery, IEnumerable<ThemeDto>>
    {
        private readonly IThemeRepository _themeRepository;

        public GetAllThemesHandler(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }



        public async Task<IEnumerable<ThemeDto>> Handle(GetAllThemesQuery request, CancellationToken cancellationToken)
        {
            var themes = await _themeRepository.GetByParentIdAsync(request.ParentThemeId, cancellationToken);

            var themeDtos = new List<ThemeDto>();
            foreach (var theme in themes)
            {
                var subThemesCount = await _themeRepository.GetSubThemesCountAsync(theme.Id, cancellationToken);

                themeDtos.Add(new ThemeDto
                {
                    Id = theme.Id,
                    Name = theme.Name,
                    ParentThemeId = theme.ParentThemeId,
                    ParentThemeName = theme.ParentTheme?.Name,
                    SubThemesCount = subThemesCount,
                    CreatedAt = null,
                    UpdatedAt = null
                });
            }

            return themeDtos;
        }
    }

    public class GetThemesByNameHandler : IRequestHandler<GetThemesByNameQuery, List<ThemeDto>>
    {
        private readonly IThemeRepository _themeRepository;
        public GetThemesByNameHandler(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }
        public async Task<List<ThemeDto>> Handle(GetThemesByNameQuery request, CancellationToken cancellationToken)
        {
            var themes = await _themeRepository.GetByNameAsync(request.Name, cancellationToken);
            var themeDtos = new List<ThemeDto>();
            foreach (var theme in themes)
            {
                themeDtos.Add(new ThemeDto
                {
                    Id = theme.Id,
                    Name = theme.Name,
                });
            }
            return themeDtos;
        }
    }
    public class GetThemeByIdHandler : IRequestHandler<GetThemeByIdQuery, ThemeDto?>
    {
        private readonly IThemeRepository _themeRepository;

        public GetThemeByIdHandler(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }

        public async Task<ThemeDto?> Handle(GetThemeByIdQuery request, CancellationToken cancellationToken)
        {
            var theme = await _themeRepository.GetByIdAsync(request.Id, cancellationToken);
            if (theme == null) return null;

            var subThemesCount = await _themeRepository.GetSubThemesCountAsync(theme.Id, cancellationToken);

            return new ThemeDto
            {
                Id = theme.Id,
                Name = theme.Name,
                ParentThemeId = theme.ParentThemeId,
                ParentThemeName = theme.ParentTheme?.Name,
                SubThemesCount = subThemesCount,
                CreatedAt = null,
                UpdatedAt = null
            };
        }
    }

    public class CreateThemeHandler : IRequestHandler<CreateThemeCommand, long>
    {
        private readonly IThemeRepository _themeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateThemeHandler(IThemeRepository themeRepository, IUnitOfWork unitOfWork)
        {
            _themeRepository = themeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateThemeCommand request, CancellationToken cancellationToken)
        {
            if (request.ParentThemeId.HasValue)
            {
                var parentTheme = await _themeRepository.GetByIdAsync(request.ParentThemeId.Value, cancellationToken);
                if (parentTheme == null)
                {
                    throw new ChronolibrisException($"Родительская тема с ID {request.ParentThemeId} не найдена", ErrorType.NotFound);
                }
            }

            var theme = new Theme
            {
                Id=0,
                Name = request.Name.Trim(),
                ParentThemeId = request.ParentThemeId
            };

            await _themeRepository.AddAsync(theme, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return theme.Id;
        }
    }

    public class UpdateThemeHandler : IRequestHandler<UpdateThemeCommand, Unit>
    {
        private readonly IThemeRepository _themeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateThemeHandler(IThemeRepository themeRepository, IUnitOfWork unitOfWork)
        {
            _themeRepository = themeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateThemeCommand request, CancellationToken ct)
        {
            var theme = await _themeRepository.GetByIdAsync(request.Id, ct);
            if (theme == null) throw new ChronolibrisException($"Тема с ID {request.Id} не найдена", ErrorType.NotFound);

            if (request.ParentThemeId.HasValue)
            {
                bool createsCycle = await _themeRepository.IsAncestorAsync(
                    request.Id,
                    request.ParentThemeId,
                    ct);

                if (createsCycle)
                    throw new ChronolibrisException(
                        "Невозможно переместить тему: выбранный родитель является потомком текущей темы",
                        ErrorType.Conflict);
            }

            if (request.ParentThemeId.HasValue)
            {
                var parentTheme = await _themeRepository.GetByIdAsync(request.ParentThemeId.Value, ct);
                if (parentTheme == null)
                {
                    throw new ChronolibrisException($"Родительская тема с ID {request.ParentThemeId} не найдена", ErrorType.NotFound);
                }
            }

            theme.Name = request.Name.Trim();
            theme.ParentThemeId = request.ParentThemeId;

            _themeRepository.Update(theme);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }

    public class DeleteThemeHandler : IRequestHandler<DeleteThemeCommand, Unit>
    {
        private readonly IThemeRepository _themeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteThemeHandler(IThemeRepository themeRepository, IUnitOfWork unitOfWork)
        {
            _themeRepository = themeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteThemeCommand request, CancellationToken cancellationToken)
        {
            var theme = await _themeRepository.GetByIdAsync(request.Id, cancellationToken);
            if (theme == null) return Unit.Value;

            var hasSubThemes = await _themeRepository.HasSubThemesAsync(request.Id, cancellationToken);
            if (hasSubThemes)
            {
                throw new ChronolibrisException(
                    "Нельзя удалить тему, у которой есть дочерние темы. Сначала удалите или переместите дочерние темы",
                    ErrorType.Unprocessable);
            }

            _themeRepository.Delete(theme);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}