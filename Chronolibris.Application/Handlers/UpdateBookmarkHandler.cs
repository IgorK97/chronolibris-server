using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    public class UpdateBookmarkHandler : IRequestHandler<UpdateBookmarkCommand, bool>
    {
        private readonly IBookmarkRepository _bookmarkRepository;

        public UpdateBookmarkHandler(IBookmarkRepository bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }

        public async Task<bool> Handle(
            UpdateBookmarkCommand request,
            CancellationToken cancellationToken)
        {
            var bookmark = await _bookmarkRepository.GetByIdAsync(request.bookmarkId);
            if(bookmark == null || bookmark.UserId != request.userId)
            {
                return false;
            }
            bookmark.Note = request.noteText;

            _bookmarkRepository.Update(bookmark);

            return true;
        }
    }
}
