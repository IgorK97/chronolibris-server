using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record AddBookToShelfCommand(long ShelfId, long BookId, long UserId)
    : IRequest;

}
