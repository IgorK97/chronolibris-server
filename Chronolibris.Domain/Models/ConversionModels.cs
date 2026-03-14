using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    /// <summary>
    /// Итог конвертации одного FB2-файла.
    /// </summary>
    public sealed class ConversionResult
    {
        /// <summary>UUID книги (из document-info или сгенерированный).</summary>
        public required string BookId { get; init; }

        /// <summary>Метаданные книги, извлечённые из FB2.</summary>
        public required BookMeta Meta { get; init; }

        /// <summary>Суммарное число элементов во всём теле книги.</summary>
        public int TotalElements { get; init; }

        /// <summary>Описание файла toc.json, сохранённого в хранилище.</summary>
        public required StoredFileInfo TocFile { get; init; }

        /// <summary>Описания фрагментных файлов (000.js, 001.js …).</summary>
        public required IReadOnlyList<StoredFileInfo> PartFiles { get; init; }

        /// <summary>Дата/время завершения конвертации (UTC).</summary>
        public DateTimeOffset CompletedAt { get; init; } = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Информация об одном файле, сохранённом в хранилище.
    /// Достаточна для записи строки в таблицу book_files / book_parts.
    /// </summary>
    public sealed class StoredFileInfo
    {
        /// <summary>UUID книги-владельца.</summary>
        public required string BookId { get; init; }

        /// <summary>Имя файла («toc.json», «000.js», «001.js» …).</summary>
        public required string FileName { get; init; }

        /// <summary>Тип файла.</summary>
        public required StoredFileType FileType { get; init; }

        /// <summary>Глобальный индекс первого элемента фрагмента (включительно). Для toc — 0.</summary>
        public int GlobalStart { get; init; }

        /// <summary>Глобальный индекс последнего элемента фрагмента (включительно). Для toc — 0.</summary>
        public int GlobalEnd { get; init; }

        /// <summary>Координата первого элемента [bodyIdx, sectionIdx, elemIdx]. Для toc — null.</summary>
        public int[]? XpStart { get; init; }

        /// <summary>Координата последнего элемента. Для toc — null.</summary>
        public int[]? XpEnd { get; init; }

        /// <summary>Размер содержимого в байтах (UTF-8).</summary>
        public long SizeBytes { get; init; }
    }

    public enum StoredFileType
    {
        Toc,
        Part
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Метаданные книги
    // ─────────────────────────────────────────────────────────────────────────────

    public sealed record BookMeta
    {
        [JsonPropertyName("Annotation")]
        public string? Annotation { get; init; }

        [JsonPropertyName("Lang")]
        public string? Lang { get; init; }

        [JsonPropertyName("Title")]
        public string? Title { get; init; }

        [JsonPropertyName("Sequences")]
        public List<string>? Sequences { get; init; }

        [JsonPropertyName("Created")]
        public string? Created { get; init; }

        [JsonPropertyName("Updated")]
        public string? Updated { get; init; }

        [JsonPropertyName("Written")]
        public WrittenInfo? Written { get; init; }

        [JsonPropertyName("Authors")]
        public List<AuthorInfo>? Authors { get; init; }

        [JsonPropertyName("ArtID")]
        public string? ArtId { get; init; }

        [JsonPropertyName("UUID")]
        public string? Uuid { get; init; }

        [JsonPropertyName("version")]
        public string? Version { get; init; }
    }


    public sealed record WrittenInfo
    {
        [JsonPropertyName("DatePublic")]
        public string? DatePublic { get; init; }

        [JsonPropertyName("Date")]
        public string? Date { get; init; }
    }

    public sealed record AuthorInfo
    {
        [JsonPropertyName("Role")]
        public string Role { get; init; } = "author";

        [JsonPropertyName("First")]
        public string? First { get; init; }

        [JsonPropertyName("Last")]
        public string? Last { get; init; }
    }

}
