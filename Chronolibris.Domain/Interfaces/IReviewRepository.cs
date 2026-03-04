using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Models;

namespace Chronolibris.Domain.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для работы с отзывами (<see cref="Review"/>).
    /// Расширяет базовый интерфейс <see cref="IGenericRepository{T}"/>.
    /// </summary>
    public interface IReviewRepository : IGenericRepository<Review>
    {
        /// <summary>
        /// Асинхронно получает все отзывы для указанной книги.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <param name="token">Токен отмены.</param>
        /// <returns>Коллекция отзывов.</returns>
        Task<List<ReviewDetailsWithVote>> GetByBookIdAsync(long bookId, long? lastId, int limit, long? userId, CancellationToken token = default);
        Task<ReviewDetailsWithVote?> GetByIdWithVotesAsync(long reviewId, long userId, CancellationToken token = default);

        Task<ReviewDetailsWithVote?> GetActiveByUserAndBookAsync(long userId, long bookId, CancellationToken token = default);

        //Task<long> CountLikesForReview(long reviewId, CancellationToken token = default);
        //Task<long> CountDislikesForReview(long reviewId, CancellationToken token = default);
        //Task<long> GetAverageForReview(long reviewId, CancellationToken token = default);

        /// <summary>
        /// Атомарно пересчитывает счетчики лайков, дизлайков и средний рейтинг для отзыва, 
        /// обновляя сущность <see cref="Review"/> в базе данных.
        /// </summary>
        /// <remarks>
        /// Этот метод должен выполнять пересчет на уровне базы данных, чтобы предотвратить гонку данных.
        /// </remarks>
        /// <param name="reviewId">Идентификатор отзыва для пересчета.</param>
        /// <param name="token">Токен отмены.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        //Task RecalculateRatingAsync(long reviewId, CancellationToken token = default);
    }
}
