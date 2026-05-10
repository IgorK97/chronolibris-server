using MediatR;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.References.Countries
{
    public class DeleteCountryHandler : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCountryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(request.Id, cancellationToken);
            if (country == null) return false;

            _unitOfWork.Countries.Delete(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}