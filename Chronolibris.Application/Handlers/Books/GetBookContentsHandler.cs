using MediatR;
using Chronolibris.Application.Requests.Books;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Handlers.Books
{
    public class GetBookContentsHandler : IRequestHandler<GetBookContentsQuery, List<ContentDto>>
    {
        private readonly IBookRepository _bookRepository;

        public GetBookContentsHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<ContentDto>> Handle(GetBookContentsQuery request, CancellationToken ct)
        {
            var contents = await _bookRepository.GetContentsWithDetailsByBookIdAsync(request.BookId, ct);

            return contents.Select(content => new ContentDto
            {
                Id = content.Id,
                Title = content.Title,
                Description = content.Description,
                CountryId = content.CountryId,
                CountryName = content.Country?.Name,
                ContentTypeId = content.ContentTypeId,
                ContentType = content.ContentType?.Name,
                LanguageId = content.LanguageId,
                LanguageName = content.Language?.Name,
                YearFrom = content.YearFrom,
                YearTo = content.YearTo,
                CreatedAt = content.CreatedAt,
                Authors = content.Participations
                    .Select(p => p.Person.Name)
                    .ToList(),
                Themes = content.Themes.Select(t => new ThemeDto
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            }).ToList();

        }
    }
}