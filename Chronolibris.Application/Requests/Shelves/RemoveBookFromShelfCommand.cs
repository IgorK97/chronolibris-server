using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{

    public record RemoveBookFromShelfCommand(long ShelfId, long BookId, long UserId)
     : IRequest;

}
