using MediatR;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests.References;
using Chronolibris.Domain.Interfaces.Repository;

namespace Chronolibris.Application.Handlers.References.Formats
{
    public class GetAllFormatsHandler : IRequestHandler<GetAllFormatsQuery, IEnumerable<FormatDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllFormatsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FormatDto>> Handle(GetAllFormatsQuery request, CancellationToken cancellationToken)
        {
            var formats = await _unitOfWork.Formats.GetAllAsync(cancellationToken);
            return formats.Select(f => new FormatDto
            {
                Id = f.Id,
                Name = f.Name
            });
        }
    }
}