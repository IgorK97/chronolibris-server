using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers.Bookmarks
{

    public class RemoveBookmarkHandler : IRequestHandler<RemoveBookmarkCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;


        public RemoveBookmarkHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<bool> Handle(RemoveBookmarkCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.Bookmarks.GetByIdAsync(request.bookmarkId, cancellationToken);
            if (existing == null)
            {
                return false;
            }

            _unitOfWork.Bookmarks.Delete(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
