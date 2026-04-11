using MediatR;
using Chronolibris.Application.Models;
using System.Collections.Generic;

namespace Chronolibris.Application.Requests.References
{
    public class GetAllFormatsQuery : IRequest<IEnumerable<FormatDto>> { }

    public class GetFormatByIdQuery : IRequest<FormatDto?>
    {
        public int Id { get; set; }
        public GetFormatByIdQuery(int id) => Id = id;
    }

    public class CreateFormatCommand : IRequest<int>
    {
        public string Name { get; set; } = string.Empty;

        public CreateFormatCommand(string name)
        {
            Name = name;
        }
    }

    public class UpdateFormatCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public UpdateFormatCommand(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class DeleteFormatCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public DeleteFormatCommand(int id) => Id = id;
    }
}