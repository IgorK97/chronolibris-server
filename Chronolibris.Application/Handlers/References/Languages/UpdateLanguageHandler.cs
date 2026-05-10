using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Languages
{
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
}
