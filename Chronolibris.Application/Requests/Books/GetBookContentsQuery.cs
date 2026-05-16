using MediatR;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Requests.Books
{
    public record GetBookContentsQuery(long BookId) : IRequest<List<ContentDto>>;
}