using MediatR;

namespace Chronolibris.Application.Requests.References.Tags
{
    public record DeleteTagRequest(long TagId) : IRequest<bool>;
}
