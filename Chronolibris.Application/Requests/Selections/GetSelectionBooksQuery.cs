using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Selections
{
    public record GetSelectionBooksQuery(long SelectionId, long? LastId, int Limit, long UserId, bool Mode)
    : IRequest<PagedResult<BookListItem>>;

}
