using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Domain.Entities;
using MediatR;

namespace Chronolibris.Application.Requests.References
{
    public record DeletePersonCommand(long Id) : IRequest;
    public record CreatePersonCommand(
    string Name,
    string Description) : IRequest<long>;
    public record UpdatePersonCommand(
    long Id,
    string Name,
    string Description) : IRequest;
    public record GetPersonByIdQuery(long Id) : IRequest<Person?>;
    public class GetAllPersonsQuery : IRequest<IEnumerable<PersonDto>> { }

}
