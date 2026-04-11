using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Requests.Books
{
    public class UpdateReadingProgressCommand : IRequest<ReadingProgressDto>
    {
        public long UserId { get; init; }
        public long BookFileId { get; init; }
        public decimal Percentage { get; init; }
        public int ParaIndex { get; set; }
    }
}
