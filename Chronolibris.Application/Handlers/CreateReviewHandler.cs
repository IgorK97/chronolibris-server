using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    /// <summary>
    /// Обработчик команды для создания нового отзыва (<see cref="Review"/>) о книге.
    /// Реализует интерфейс <see cref="IRequestHandler{TRequest, TResponse}"/> 
    /// для обработки <see cref="CreateReviewCommand"/> и возврата идентификатора отзыва (<see cref="long"/>).
    /// </summary>
    public class CreateReviewHandler : IRequestHandler<CreateReviewCommand, long>
    {
        /// <summary>
        /// Приватное поле только для чтения для доступа к Unit of Work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="CreateReviewHandler"/>.
        /// </summary>
        /// <param name="unitOfWork">Интерфейс Unit of Work для взаимодействия с базой данных.</param>
        public CreateReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Обрабатывает команду создания нового отзыва.
        /// </summary>
        /// <remarks>
        /// Создает новую сущность <see cref="Review"/> из данных команды, 
        /// добавляет ее через репозиторий и сохраняет изменения в базе данных.
        /// В конце возвращает сгенерированный базой данных идентификатор нового отзыва.
        /// </remarks>
        /// <param name="request">Объект команды, содержащий данные для нового отзыва.</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию. 
        /// Результат задачи — <see cref="long"/>, представляющий идентификатор нового отзыва.
        /// </returns>
        public async Task<long> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {

            var existing = await _unitOfWork.Reviews.GetActiveByUserAndBookAsync(request.UserId, request.BookId, cancellationToken);
            if (existing != null)
                throw new Exception("Пользователь уже оставил отзыв на эту книгу.");
            if(request.Score < 1 || request.Score > 5)
                throw new Exception("Оценка должна быть от 1 до 5.");
            if (request.ReviewText != null && (request.ReviewText.Length < 120 || string.IsNullOrWhiteSpace(request.ReviewText.Trim())))
                throw new Exception("Текст отзыва должен быть не менее 120 символов.");

            var review = new Review
            {
                BookId = request.BookId,
                UserId = request.UserId,
                // Используем оператор объединения с null (??) для обеспечения не-null значений для строк
                //Title = request.Title ?? "",
                ReviewText = request.ReviewText ?? null,
                Score = request.Score,
                CreatedAt = DateTime.UtcNow,
                //AverageRating = 0,
                //DislikesCount = 0,
                Id = 0,
                ReviewStatusId = request.ReviewText != null ? 1 : 2,
                //LikesCount = 0,
                //Name = request.UserName ?? "",
            };

            //var book = await _unitOfWork.Books.GetByIdAsync(request.BookId, cancellationToken);
            //long newRatingsCount = book.RatingsCount + 1;

            // b) Обновление среднего рейтинга (AverageRating)
            // Формула для пересчета среднего: (СтарыйСредний * СтароеКоличество + НоваяОценка) / НовоеКоличество
            //decimal oldTotalScore = book.AverageRating * book.RatingsCount;
            //decimal newAverageRating = (oldTotalScore + request.Score) / newRatingsCount;

            //book.AverageRating = newAverageRating;
            //book.RatingsCount = newRatingsCount;

            // c) +1 к количеству отзывов (ReviewsCount), если есть текст отзыва
            //if (!string.IsNullOrWhiteSpace(request.Description) || !string.IsNullOrWhiteSpace(request.Title))
            //{
            //    book.ReviewsCount++;
            //}
            // Добавление новой сущности в контекст отслеживания (не сохраняет в БД)
            await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
            //_unitOfWork.Books.Update(book);
            // Сохранение изменений в базе данных и присвоение сущности сгенерированного Id
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Возврат сгенерированного идентификатора
            return review.Id;
        }
    }
}
