using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Themes
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
            var themes = await _themeRepository.GetByParentIdAsync(request.ParentThemeId, cancellationToken); //потом усовершенствовать, сразу пусть возвращает дто вместе
            //с подсчетом

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
                    SubThemesCount = subThemesCount
                });
            }

            return themeDtos;
        }
    }

}
