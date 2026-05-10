using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Countries
{
    public class CreateCountryHandler : IRequestHandler<CreateCountryCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCountryHandler(IGenericRepository<Country> repository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = new Country
            {
                Id = 0,
                Name = request.Name.Trim()
            };

            await _unitOfWork.Countries.AddAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return country.Id;
        }
    }
}
