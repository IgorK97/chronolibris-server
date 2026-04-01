using System.Collections.Generic;
using Chronolibris.Application.Requests;
using Chronolibris.Domain.Models; // для PersonRoleFilter

namespace Chronolibris.Application.Requests
{
    /// <summary>
    /// Тело запроса для создания книги (application/json).
    /// Обложка передаётся как Base64-строка.
    /// </summary>
    public class CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CountryId { get; set; }
        public int LanguageId { get; set; }
        public int? Year { get; set; }
        public string? ISBN { get; set; }
        public string? Bbk { get; set; }
        public string? Udk { get; set; }
        public string? Source { get; set; }

        /// <summary>
        /// Файл обложки в формате Base64. Обязателен при создании.
        /// Пример: "data:image/jpeg;base64,/9j/4AAQ..."  или просто Base64 без префикса.
        /// </summary>
        public string CoverBase64 { get; set; } = string.Empty;

        /// <summary>MIME-тип обложки, напр. "image/jpeg".</summary>
        public string CoverContentType { get; set; } = "image/jpeg";

        /// <summary>Оригинальное имя файла, напр. "cover.jpg". Нужно для расширения.</summary>
        public string CoverFileName { get; set; } = "cover";

        public bool IsAvailable { get; set; } = true;
        public bool IsReviewable { get; set; }
        public int? PublisherId { get; set; }
        public int? SeriesId { get; set; }
        public List<PersonRoleFilter>? PersonFilters { get; set; }
        public List<int>? ThemeIds { get; set; }
    }

    /// <summary>
    /// Тело запроса для обновления книги (application/json).
    ///
    /// Логика частичного обновления:
    ///   - Простые nullable-поля (CountryId, LanguageId): null → не обновлять.
    ///   - Поля с флагом *Provided: обновляем только если *Provided == true
    ///     (позволяет явно передать null, чтобы сбросить значение).
    ///   - CoverBase64: если не null/пустая → перезаписать файл в MinIO;
    ///     путь в БД не меняется.
    /// </summary>
    public class UpdateBookRequest
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Обновляются только если не null
        public int? CountryId { get; set; }
        public int? LanguageId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsReviewable { get; set; }

        // Поля с явным флагом наличия
        public int? Year { get; set; }
        public bool YearProvided { get; set; }

        public string? ISBN { get; set; }
        public bool IsbnProvided { get; set; }

        public string? Bbk { get; set; }
        public bool BbkProvided { get; set; }

        public string? Udk { get; set; }
        public bool UdkProvided { get; set; }

        public string? Source { get; set; }
        public bool SourceProvided { get; set; }

        public int? PublisherId { get; set; }
        public bool PublisherIdProvided { get; set; }

        public int? SeriesId { get; set; }
        public bool SeriesIdProvided { get; set; }

        /// <summary>
        /// Новая обложка в Base64. Если null или пустая — обложка не меняется.
        /// Если передана — файл перезаписывается в MinIO; путь в БД остаётся прежним.
        /// </summary>
        public string? CoverBase64 { get; set; }
        public string? CoverContentType { get; set; }
        public string? CoverFileName { get; set; }

        public List<PersonRoleFilter>? PersonFilters { get; set; }
        public List<int>? ThemeIds { get; set; }
    }
}