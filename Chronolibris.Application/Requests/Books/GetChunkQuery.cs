using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public record GetChunkQuery(long BookFileId, string ChunkIndex, long UserId, string Role) : IRequest<string?>;
}
