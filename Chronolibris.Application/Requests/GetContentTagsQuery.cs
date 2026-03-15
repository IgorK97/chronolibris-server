using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests
{
    public record GetContentTagsQuery(long ContentId) : IRequest<List<TagDetails>>;
}
