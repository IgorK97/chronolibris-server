using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Themes
{
    public class CreateThemeHandler : IRequestHandler<CreateThemeCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateThemeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateThemeCommand request, CancellationToken cancellationToken)
        {
            if (request.ParentThemeId.HasValue)
            {
                var parentTheme = await _unitOfWork.Themes.GetByIdAsync(request.ParentThemeId.Value, cancellationToken);
                if (parentTheme == null)
                {
                    throw new ChronolibrisException($"Родительская тема с ID {request.ParentThemeId} не найдена", ErrorType.NotFound);
                }
            }

            var theme = new Theme
            {
                Id = 0,
                Name = request.Name.Trim(),
                ParentThemeId = request.ParentThemeId
            };

            await _unitOfWork.Themes.AddAsync(theme, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return theme.Id;
        }
    }
}
