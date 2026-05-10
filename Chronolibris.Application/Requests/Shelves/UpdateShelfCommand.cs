using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record UpdateShelfCommand(long UserId, long ShelfId, string Name) : IRequest<Unit>;

}
