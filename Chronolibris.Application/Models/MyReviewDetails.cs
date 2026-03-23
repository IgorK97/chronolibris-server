using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Models
{
    public class MyReviewDetails
    {
        /// <summary>
        /// Обязательный уникальный идентификатор отзыва.
        /// </summary>
        public required long Id { get; set; }
        public required string UserName { get; set; }
        //public long UserId { get; set; }
        //public long BookId { get; set; }

        /// <summary>
        /// Обязательный заголовок отзыва (рецензии).
        /// </summary>
        //public required string Title { get; set; }

        /// <summary>
        /// Обязательное отображаемое имя пользователя, оставившего отзыв.
        /// </summary>
        //public required string UserName { get; set; }

        /// <summary>
        /// Обязательный полный текст отзыва.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Обязательная оценка, выставленная пользователем книге в этом отзыве 
        /// (например, по шкале от 1 до 5).
        /// </summary>
        public required short Score { get; set; }

        /// <summary>
        /// Обязательный общий средний рейтинг книги, актуальный на момент публикации отзыва или просмотра.
        /// </summary>
        //public required decimal AverageRating { get; set; }

        /// <summary>
        /// Обязательное количество положительных оценок (лайков), полученных этим отзывом от других пользователей.
        /// </summary>
        public required long LikesCount { get; set; }

        /// <summary>
        /// Обязательное количество отрицательных оценок (дизлайков), полученных этим отзывом от других пользователей.
        /// </summary>
        public required long DislikesCount { get; set; }

        /// <summary>
        /// Обязательная дата и время создания отзыва.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Указывает, как текущий аутентифицированный пользователь оценил этот отзыв.
        /// <c>true</c> для лайка, <c>false</c> для дизлайка. <c>null</c>, если пользователь еще не голосовал.
        /// </summary>
        public bool? UserVote { get; set; }
        //public required string Status { get; set; }
    }
}
