using MediatR;

namespace Chronolibris.Application.Requests.Contents
{
    public record AddTagToContentCommand(
       long ContentId,
       long TagId
   ) : IRequest<bool>;
}
