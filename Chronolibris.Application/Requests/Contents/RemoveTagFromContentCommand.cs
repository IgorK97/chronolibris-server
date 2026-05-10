using MediatR;

namespace Chronolibris.Application.Requests.Contents
{
    public record RemoveTagFromContentCommand(
        long ContentId,
        long TagId
    ) : IRequest<bool>;
}
