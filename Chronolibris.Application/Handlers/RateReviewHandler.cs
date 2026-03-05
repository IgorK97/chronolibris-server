using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Models;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Interfaces;
using MediatR;

namespace Chronolibris.Application.Handlers
{
    /// <summary>
    /// Обработчик команды для оценки отзыва пользователем (Like, Dislike или снятие оценки).
    /// Реализует интерфейс <see cref="IRequestHandler{TRequest, TResponse}"/>
    /// для обработки <see cref="RateReviewCommand"/> и возврата обновленного <see cref="ReviewDetails"/> DTO.
    /// </summary>
    public class RateReviewHandler : IRequestHandler<RateReviewCommand, ReviewDetails?>
    {
        /// <summary>
        /// Приватное поле только для чтения для доступа к паттерну Unit of Work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="RateReviewHandler"/>.
        /// </summary>
        /// <param name="unitOfWork">Интерфейс Unit of Work для взаимодействия с базой данных.</param>
        public RateReviewHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Обрабатывает команду оценки отзыва.
        /// </summary>
        /// <remarks>
        /// Логика команды включает многоэтапный процесс для обеспечения согласованности данных:
        /// <list type="number">
        /// <item>Получение отзыва для проверки существования и отслеживания.</item>
        /// <item>Обработка изменения/удаления/добавления пользовательской оценки (<c>ReviewsRating</c>).</item>
        /// <item>**Первое сохранение** (<c>SaveChangesAsync</c>): Фиксирует изменение <c>ReviewsRating</c>.</item>
        /// <item>**Атомарный пересчет** (<c>RecalculateRatingAsync</c>): Выполняет безопасное, не зависящее от ORM, обновление счетчиков <c>Review</c> в БД, предотвращая Lost Update.</item>
        /// <item>**Отсоединение и Перезагрузка:** Отключает старую сущность <c>review</c> (`Detach`) и загружает ее заново (`GetByIdAsync`), чтобы получить актуальные счетчики из БД.</item>
        /// </list>
        /// </remarks>
        /// <param name="request">Объект команды, содержащий <c>ReviewId</c>, <c>UserId</c> и новую <c>Score</c> (1: Like, -1: Dislike, 0: Remove).</param>
        /// <param name="cancellationToken">Токен отмены для асинхронной операции.</param>
        /// <returns>
        /// Задача, представляющая асинхронную операцию.
        /// Результат задачи — обновленный объект <see cref="ReviewDetails"/> с актуальными счетчиками, или <c>null</c>, если отзыв не найден.
        /// </returns>
        public async Task<ReviewDetails?> Handle(RateReviewCommand request, CancellationToken cancellationToken)
        {
            if (request.Score != 1 && request.Score != -1)
                throw new Exception("Неверная оценка");

            var review = await _unitOfWork.Reviews.GetByIdWithVotesAsync(request.ReviewId, request.UserId, cancellationToken);
            if (review == null)
                return null;

            
            var rating = await _unitOfWork.ReviewsRatings.GetReviewsRatingByUserIdAsync(request.ReviewId,
                request.UserId, cancellationToken);

            if (rating is null)
            {
                rating = new ReviewsReaction
                {
                    Id = 0,
                    ReviewId = request.ReviewId,
                    ReactionType = request.Score,
                    UserId = request.UserId,
                };
            }
            else
            {
                rating.ReactionType = request.Score == rating.ReactionType ? (short)0 : request.Score;
            }

                //if (request.Score == 0) // Снятие оценки
                //{
                //    if (rating != null)
                //        _unitOfWork.ReviewsRatings.Delete(rating);
                //}
                //else // Установка или изменение оценки
                //{
                //    if (rating == null)
                //    {
                //        rating = new ReviewsReaction
                //        {
                //            Id = 0,
                //            ReviewId = request.ReviewId,
                //            ReactionType = request.Score,
                //            UserId = request.UserId,
                //        };
                //        await _unitOfWork.ReviewsRatings.AddAsync(rating, cancellationToken);
                //    }
                //    else
                //    {
                //        rating.ReactionType = request.Score;
                //    }
                //}


                await _unitOfWork.SaveChangesAsync(cancellationToken);

            //await _unitOfWork.Reviews.RecalculateRatingAsync(request.ReviewId, cancellationToken);


            _unitOfWork.Reviews.Detach(review.Review);
            var newReview = await _unitOfWork.Reviews.GetByIdAsync(request.ReviewId, cancellationToken);

            if (review == null) return null;


            return new ReviewDetails
            {
                Id = review.Review.Id,

                //AverageRating = review.AverageRating,
                DislikesCount = review.DislikesCount,
                LikesCount = review.LikesCount,


                CreatedAt = review.Review.CreatedAt,
                Score = review.Review.Score,
                Text = review.Review.ReviewText,
                //Title = review.Title,
                //UserName = review.Name,
                UserVote = request.Score switch
                {
                    1 => true,
                    -1 => false,
                    _ => null
                }
            };
        }



    }
}
