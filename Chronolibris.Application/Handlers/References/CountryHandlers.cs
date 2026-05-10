using MediatR;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Handlers.References
{
    public class GetAllCountriesHandler : IRequestHandler<GetAllCountriesQuery, IEnumerable<CountryDto>>
    {
        private readonly IGenericRepository<Country> _repository;

        public GetAllCountriesHandler(IGenericRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            var countries = await _repository.GetAllAsync(cancellationToken);
            return countries.Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(c=>c.Name);
        }
    }

    public class GetCountryByIdHandler : IRequestHandler<GetCountryByIdQuery, CountryDto?>
    {
        private readonly IGenericRepository<Country> _repository;

        public GetCountryByIdHandler(IGenericRepository<Country> repository)
        {
            _repository = repository;
        }

        public async Task<CountryDto?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var country = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (country == null) return null;

            return new CountryDto
            {
                Id = country.Id,
                Name = country.Name
            };
        }
    }

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