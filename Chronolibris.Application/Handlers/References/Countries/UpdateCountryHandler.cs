using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Countries
{
    public class UpdateCountryHandler : IRequestHandler<UpdateCountryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCountryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _unitOfWork.Countries.GetByIdAsync(request.Id, cancellationToken);
            if (country == null) return false;

            country.Name = request.Name.Trim();

            _unitOfWork.Countries.Update(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
