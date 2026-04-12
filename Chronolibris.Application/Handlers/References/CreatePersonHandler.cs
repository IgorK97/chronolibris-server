using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References
{
    public record CreatePersonCommand(
    string Name,
    string Description) : IRequest<long>;

    public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, long>
    {
        private readonly IGenericRepository<Person> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePersonHandler(IGenericRepository<Person> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreatePersonCommand request, CancellationToken token)
        {


            var person = new Person
            {
                Id = 0,
                Name = request.Name,
                Description = request.Description,
                //ImagePath = imagePath,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(person, token);
            await _unitOfWork.SaveChangesAsync(token);

            return person.Id;
        }
    }
}
