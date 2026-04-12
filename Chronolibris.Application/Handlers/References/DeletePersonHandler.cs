using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers.References
{
    public record DeletePersonCommand(long Id) : IRequest;

    public class DeletePersonHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IGenericRepository<Person> _repository;
        private readonly IStorageService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePersonHandler(IGenericRepository<Person> repository, IStorageService fileService, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeletePersonCommand request, CancellationToken token)
        {
            var person = await _repository.GetByIdAsync(request.Id, token);
            if (person == null) return;
            _repository.Delete(person);
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}
