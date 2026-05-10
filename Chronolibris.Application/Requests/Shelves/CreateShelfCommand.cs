using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record CreateShelfCommand(long UserId, string Name) : IRequest<long>;
}
