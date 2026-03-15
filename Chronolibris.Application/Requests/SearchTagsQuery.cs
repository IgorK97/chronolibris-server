using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests
{
    public record SearchTagsQuery(
     string SearchTerm,
     long? TagTypeId = null,
     int Limit = 5
 ) : IRequest<List<TagDetails>>;
}
