using Chronolibris.Application.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Users
{
    public record GetUserShelvesQuery(long UserId)
    : IRequest<IEnumerable<ShelfDetails>>;

}
