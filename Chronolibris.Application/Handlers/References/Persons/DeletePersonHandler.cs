using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Persons
{

    public class DeletePersonHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePersonHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeletePersonCommand request, CancellationToken token)
        {
            var person = await _unitOfWork.Persons.GetByIdAsync(request.Id, token);
            if (person == null) return;
            _unitOfWork.Persons.Delete(person);
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
