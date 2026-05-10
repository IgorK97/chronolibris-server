using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Contents
{
    public record GetContentTagsQuery(long ContentId) : IRequest<List<TagDetails>>;
}
