using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.References.Publishers
{
    public class UpdatePublisherHandler : IRequestHandler<UpdatePublisherCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePublisherHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(request.Id, cancellationToken);
            if (publisher == null) return false; //и здесь тоже самое

            publisher.Name = request.Name.Trim();
            publisher.Description = request.Description;

            _unitOfWork.Publishers.Update(publisher);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
