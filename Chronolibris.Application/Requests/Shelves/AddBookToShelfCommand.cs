using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Chronolibris.Application.Requests.Shelves
{
    public record AddBookToShelfCommand(long ShelfId, long BookId)
    : IRequest<bool>;

}
