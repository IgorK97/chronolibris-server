using MediatR;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.References.Languages
{
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
            if (language == null) return false; //то же самое

            _unitOfWork.Languages.Delete(language);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
