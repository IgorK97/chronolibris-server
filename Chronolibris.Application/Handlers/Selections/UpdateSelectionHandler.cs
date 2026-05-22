using System.Xml.Linq;
using Chronolibris.Application.Requests.Selections;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Selections
{
    public class UpdateSelectionHandler : IRequestHandler<UpdateSelectionRequest, bool>
    {
        private readonly IUnitOfWork uow;

        public UpdateSelectionHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<bool> Handle(UpdateSelectionRequest request, CancellationToken ct)
        {
            var selection = await uow.Selections.GetByIdAsync(request.SelectionId, ct);
            if(selection == null)return false;
            if (request.Name != null) selection.Name = request.Name;
            if (request.Description != null) selection.Description = request.Description;
            if (request.IsActive.HasValue) selection.IsActive = request.IsActive.Value;
            selection.UpdatedAt = DateTime.UtcNow;
            //selection.UpdatedBy = request.UserId;

            await uow.SaveChangesAsync(ct);

            return true;
        }
    }
}
