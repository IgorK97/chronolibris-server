using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References
{
    public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePersonHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreatePersonCommand request, CancellationToken token)
        {


            var person = new Person
            {
                Id = 0,
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                //ImagePath = imagePath,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Persons.AddAsync(person, token);
            await _unitOfWork.SaveChangesAsync(token);

            return person.Id;
        }
    }
}
