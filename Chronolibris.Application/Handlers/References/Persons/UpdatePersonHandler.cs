using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;


namespace Chronolibris.Application.Handlers.References.Persons
{
    public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand>
    {
        private readonly IStorageService _fileService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePersonHandler(IStorageService fileService, IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdatePersonCommand request, CancellationToken token)
        {
            var person = await _unitOfWork.Persons.GetByIdAsync(request.Id, token);
            if (person == null) throw new KeyNotFoundException("Персоналия не найдена");

            person.Name = request.Name.Trim();
            person.Description = request.Description;


            _unitOfWork.Persons.Update(person);
            await _unitOfWork.SaveChangesAsync(token);
        }
    }
}