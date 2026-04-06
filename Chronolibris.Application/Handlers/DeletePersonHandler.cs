using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using Chronolibris.Domain.Interfaces.Services;
using MediatR;

namespace Chronolibris.Application.Handlers
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

            // Удаляем из БД
            _repository.Delete(person);
            await _unitOfWork.SaveChangesAsync(token);

            // Удаляем файл из MinIO
            //await _fileService.DeleteFileAsync(person.ImagePath, token);
        }
    }
}
