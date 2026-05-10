using MediatR;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Handlers.References.Themes
{
    public class DeleteThemeHandler : IRequestHandler<DeleteThemeCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteThemeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteThemeCommand request, CancellationToken cancellationToken)
        {
            var theme = await _unitOfWork.Themes.GetByIdAsync(request.Id, cancellationToken);
            if (theme == null) return Unit.Value;

            var hasSubThemes = await _unitOfWork.Themes.HasSubThemesAsync(request.Id, cancellationToken);
            if (hasSubThemes)
            {
                throw new ChronolibrisException(
                    "Нельзя удалить тему, у которой есть дочерние темы. Сначала удалите дочерние темы",
                    ErrorType.Unprocessable);
            }

            _unitOfWork.Themes.Delete(theme);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}