using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

public record UpdatePersonCommand(
    long Id,
    string Name,
    string Description) : IRequest;

public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand>
{
    private readonly IGenericRepository<Person> _repository;
    private readonly IStorageService _fileService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePersonHandler(IGenericRepository<Person> repository, IStorageService fileService, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _fileService = fileService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdatePersonCommand request, CancellationToken token)
    {
        var person = await _repository.GetByIdAsync(request.Id, token);
        if (person == null) throw new KeyNotFoundException("Person not found");

        person.Name = request.Name;
        person.Description = request.Description;


        _repository.Update(person);
        await _unitOfWork.SaveChangesAsync(token);
    }
}