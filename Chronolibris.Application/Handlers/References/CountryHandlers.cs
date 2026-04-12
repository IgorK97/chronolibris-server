using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

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
            });
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
        //private readonly IGenericRepository<Country> _repository;
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
                Name = request.Name
            };

            await _unitOfWork.Countries.AddAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return country.Id;
        }
    }

    public class UpdateCountryHandler : IRequestHandler<UpdateCountryCommand, bool>
    {
        private readonly IGenericRepository<Country> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCountryHandler(IGenericRepository<Country> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (country == null) return false;

            country.Name = request.Name;

            _repository.Update(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    public class DeleteCountryHandler : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly IGenericRepository<Country> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCountryHandler(IGenericRepository<Country> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (country == null) return false;

            _repository.Delete(country);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}