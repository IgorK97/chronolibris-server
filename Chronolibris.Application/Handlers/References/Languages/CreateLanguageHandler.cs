using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Languages
{
    public class CreateLanguageHandler : IRequestHandler<CreateLanguageCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLanguageHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
        {
            var language = new Language
            {
                Id = 0,
                Name = request.Name.Trim(),
            };

            await _unitOfWork.Languages.AddAsync(language, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return language.Id;
        }
    }
}
