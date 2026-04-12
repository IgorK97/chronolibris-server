using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References
{
    public class GetAllPersonsQuery : IRequest<IEnumerable<PersonDto>> { }
    public class GetAllPersonsHandler : IRequestHandler<GetAllPersonsQuery, IEnumerable<PersonDto>>
    {
        private readonly IGenericRepository<Person> _repository;

        public GetAllPersonsHandler(IGenericRepository<Person> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PersonDto>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
        {
            var persons = await _repository.GetAllAsync(cancellationToken);
            return persons.OrderBy(p=>p.Name).Select(p => new PersonDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                //ImageUrl = p.ImagePath,
                CreatedAt = p.CreatedAt,
                //UpdatedAt = p.UpdatedAt
            });
        }
    }
}
