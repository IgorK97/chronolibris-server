using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Models
{
    /// <summary>
    /// Представляет подробную модель данных для подборки или тематической коллекции книг (Selection).
    /// Используется для отображения названия, описания и идентификатора конкретной подборки.
    /// </summary>
    public class SelectionDetails
    {
        /// <summary>
        /// Обязательный уникальный идентификатор подборки.
        /// </summary>
        public required long Id { get; set; }

        /// <summary>
        /// Обязательное название подборки (например, "Лучшие детективы 2024 года").
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Обязательное подробное описание, объясняющее содержание и цель подборки.
        /// </summary>
        public required string Description { get; set; }

        //public required int SelectionTypeId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt{get;set;}
        public long? BooksCount { get; set; }
        public bool IsActive { get; set; }
        
    }
}
