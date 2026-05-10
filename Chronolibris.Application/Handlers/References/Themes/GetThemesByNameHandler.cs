using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Themes
{
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
            var themeDtos = themes.Select(th => new ThemeDto { Id = th.Id, Name = th.Name }).ToList();
            return themeDtos;
        }
    }
}
