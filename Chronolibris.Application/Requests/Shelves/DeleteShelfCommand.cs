using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record DeleteShelfCommand(long UserId, long ShelfId) : IRequest<Unit>;

}
