using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Queries;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса для получения детальной информации о книге.
    /// Реализует интерфейс <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// для обработки <see cref="GetBookMetadataQuery"/> и возврата <see cref="BookDetails"/> DTO.
    /// </summary>
    public class GetBookDetailsHandler : IRequestHandler<GetBookMetadataQuery, BookDetails?>
    {
        /// <summary>
        /// Приватное поле только для чтения для доступа к репозиторию книг.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GetBookDetailsHandler"/>.
        /// </summary>
        /// <param name="bookRepository">Интерфейс репозитория для доступа к данным книг.</param>
        public GetBookDetailsHandler(IUnitOfWork uow)
        {
            unitOfWork = uow;
        }

        /// <summary>
        /// Обрабатывает запрос на получение метаданных и деталей книги.
        /// </summary>
        /// <remarks>
        /// 1. Извлекает книгу и связанные сущности (участники, издатель, страна, язык) из базы данных.
        /// 2. Если книга не найдена, возвращает <c>null</c>.
        /// 3. Группирует участников (авторов, редакторов и т.д.) по их роли.
        /// 4. Преобразует сущность <c>Book</c> и связанные данные в объект <see cref="BookDetails"/> DTO.
        /// </remarks>
        /// <param name="request">Запрос, содержащий идентификатор книги (<c>bookId</c>).</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. 
        /// Результат задачи — объект <see cref="BookDetails"/>, содержащий детали книги, 
        /// или <c>null</c>, если книга не найдена.
        /// </returns>
        public async Task<BookDetails?> Handle(GetBookMetadataQuery request, CancellationToken cancellationToken)
        {
            // Получение книги с необходимыми связанными сущностями из репозитория
            var book = await unitOfWork.Books.GetBookWithRelationsAsync(request.bookId, cancellationToken);
            if (book == null)
                return null;

            //var bookParticipations = book.Participations;
            //var contentParticipations = book.BookContents
            //    .SelectMany(bc => bc.Content.Participations)
            //    .ToList();

            //var allParticipations = bookParticipations
            //    .Concat(contentParticipations) 
            //    .ToList();

            var bookParticipations = book.Participations
                .Select(p => new { p.PersonId, p.PersonRoleId, p.Person });

            var contentParticipations = book.BookContents
                .SelectMany(bc => bc.Content.Participations)
                .Select(p => new { p.PersonId, p.PersonRoleId, p.Person });

            var allParticipations = bookParticipations
                .Concat(contentParticipations)
                .ToList();

            var allThemes = book.BookContents
                .SelectMany(bc => bc.Content.Themes)
                .ToList();

            var participantsGrouped = allParticipations
                .DistinctBy(p => new { p.PersonRoleId, p.PersonId })
                .GroupBy(p => p.PersonRoleId)
                .Select(g => new BookPersonGroupDetails
                {
                    Role = g.Key,
                    // RoleName = g.First().PersonRole.Name, 
                    Persons = g.Select(p => new PersonDetails
                    {
                        Id = p.PersonId,
                        FullName = p.Person.Name
                    }).ToList()
                })
                .ToList();

            bool isFavorite = await unitOfWork.Shelves.IsInFavorite(request.userId, request.bookId);
            bool isRead = await unitOfWork.Shelves.IsRead(request.userId, request.bookId);

            // Создание DTO (Data Transfer Object) BookDetails
            var dto = new BookDetails
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Year = book.Year,
                ISBN = book.ISBN,
                AverageRating = book.AverageRating,
                RatingsCount = book.RatingsCount,
                ReviewsCount = book.ReviewsCount,
                IsAvailable = book.IsAvailable,
                IsFavorite=isFavorite,
                IsRead = isRead,
                // Проверка на null для Publisher перед маппингом
                Publisher = book.Publisher != null ? new PublisherDetails
                {
                    Id = book.Publisher.Id,
                    Name = book.Publisher.Name
                } : null,

                // Маппинг простых связанных свойств
                Country = book.Country.Name,
                Language = book.Language.Name,

                // Присвоение сгруппированных участников
                Participants = participantsGrouped,
                //CoverUri = _cdnService.GetCoverUrl(book.CoverPath),
                CoverUri = book.CoverPath,
                Themes = allThemes.Select(th =>new ThemeDetails { Id = th.Id, Name  = th.Name }).ToList(),
                
            };

            return dto;

        }
    }
}
