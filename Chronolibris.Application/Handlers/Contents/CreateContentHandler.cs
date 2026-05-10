using Chronolibris.Application.Requests.Contents;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces.Repository;
using MediatR;

namespace Chronolibris.Application.Handlers.Contents
{
    public class CreateContentHandler : IRequestHandler<CreateContentCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateContentHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            var content = new Content
            {
                Id = 0,
                Title = request.Title,
                Description = request.Description,
                CountryId = request.CountryId,
                ContentTypeId = request.ContentTypeId,
                LanguageId = request.LanguageId,
                YearFrom = request.YearFrom,
                YearTo = request.YearTo,
                CreatedAt = DateTime.UtcNow,
                Participations = new List<ContentParticipation>(),
                Themes = new List<Theme>()
            };

            if (request.PersonFilters != null)
                _unitOfWork.Contents.SyncParticipations(content, request.PersonFilters);

            if (request.ThemeIds != null)
            {
                _unitOfWork.Contents.SyncThemes(content, request.ThemeIds);
            }


            await _unitOfWork.Contents.AddAsync(content, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return content.Id;
        }
    }

}
