using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Themes
{
    public class UpdateThemeHandler : IRequestHandler<UpdateThemeCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateThemeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateThemeCommand request, CancellationToken ct)
        {
            var theme = await _unitOfWork.Themes.GetByIdAsync(request.Id, ct);
            if (theme == null) throw new ChronolibrisException("Тема не найдена", ErrorType.NotFound);

            if (request.ParentThemeId.HasValue)
            {
                var parentTheme = await _unitOfWork.Themes.GetByIdAsync(request.ParentThemeId.Value, ct);
                if (parentTheme == null)
                {
                    throw new ChronolibrisException("Родительская тема не найдена", ErrorType.NotFound);
                }
            }

            theme.Name = request.Name.Trim();
            theme.ParentThemeId = request.ParentThemeId;

            _unitOfWork.Themes.Update(theme);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }

}
