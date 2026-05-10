using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Themes
{
    public class GetThemeByIdHandler : IRequestHandler<GetThemeByIdQuery, ThemeDto?>
    {
        private readonly IThemeRepository _themeRepository;

        public GetThemeByIdHandler(IThemeRepository themeRepository)
        {
            _themeRepository = themeRepository;
        }

        public async Task<ThemeDto?> Handle(GetThemeByIdQuery request, CancellationToken cancellationToken)
        {
            var theme = await _themeRepository.GetByIdAsync(request.Id, cancellationToken); //потом здесь тоже подправить
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
}
