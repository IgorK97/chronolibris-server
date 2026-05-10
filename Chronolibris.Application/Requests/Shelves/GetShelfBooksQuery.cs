using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record GetShelfBooksQuery(long ShelfId, long? LastId, int Limit, long UserId)
    : IRequest<PagedResult<BookListItem>>;

}
