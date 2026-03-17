using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Models
{
    /// <summary>
    /// Представляет модель данных для закладки (Bookmark), созданной пользователем в книге.
    /// Используется для передачи деталей закладки в приложении.
    /// </summary>
    public class BookmarkDetails
    {
        /// <summary>
        /// Уникальный идентификатор закладки.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Текст или местоположение закладки, например, номер страницы,
        /// цитата или точное положение в электронном файле.
        /// </summary>
        public int ParaIndex { get; set; }
        public string? Note { get; set; }
        public required long BookFileId { get; set; }

        /// <summary>
        /// Дата и время создания закладки.
        /// </summary>
        public required DateTime CreatedAt { get; set; }
    }
}
