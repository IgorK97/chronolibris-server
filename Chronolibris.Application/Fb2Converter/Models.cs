using System.Text.Json.Serialization;

namespace Chronolibris.Application.Fb2Converter
{
        // ─────────────────────────────────────────────────────────────────────────────
    // Параметры конвертации
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Настройки процесса конвертации.
    /// </summary>
    public sealed class ConversionOptions
    {
        /// <summary>
        /// Целевое число элементов (абзацев, разрывов, заголовков) в одном фрагменте.
        /// По умолчанию 88 — как в образце.
        /// </summary>
        public int TargetPartSize { get; init; } = 88;

        /// <summary>
        /// Версия формата toc.json.
        /// </summary>
        public string FormatVersion { get; init; } = "1.1";
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Результат конвертации (возвращается вызывающей стороне для записи в БД)
    // ─────────────────────────────────────────────────────────────────────────────

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

    // ─────────────────────────────────────────────────────────────────────────────
    // Внутренние модели toc.json
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>Корень toc.json.</summary>
    public sealed class TocDocument
    {
        [JsonPropertyName("Meta")]
        public required BookMeta Meta { get; init; }

        [JsonPropertyName("full_length")]
        public int FullLength { get; set; }

        [JsonPropertyName("Body")]
        public required List<TocBodyEntry> Body { get; init; }

        [JsonPropertyName("Parts")]
        public required List<TocPartEntry> Parts { get; init; }
    }

    /// <summary>Запись верхнего уровня Body: описывает всю книгу или её том.</summary>
    public sealed class TocBodyEntry
    {
        [JsonPropertyName("s")]
        public int S { get; set; }

        [JsonPropertyName("e")]
        public int E { get; set; }

        [JsonPropertyName("t")]
        public string? T { get; set; }

        [JsonPropertyName("c")]
        public List<TocChapterEntry>? C { get; set; }
    }

    /// <summary>Запись одной главы / раздела в Body.</summary>
    public sealed class TocChapterEntry
    {
        [JsonPropertyName("s")]
        public int S { get; set; }

        [JsonPropertyName("e")]
        public int E { get; set; }

        [JsonPropertyName("t")]
        public string? T { get; set; }
    }

    /// <summary>Запись одного фрагментного файла в Parts.</summary>
    public sealed class TocPartEntry
    {
        [JsonPropertyName("s")]
        public int S { get; set; }

        [JsonPropertyName("e")]
        public int E { get; set; }

        [JsonPropertyName("xps")]
        public required int[] Xps { get; set; }

        [JsonPropertyName("xpe")]
        public required int[] Xpe { get; set; }

        [JsonPropertyName("url")]
        public required string Url { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Элементы фрагмента (*.js)
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Один элемент JSON-фрагмента.
    /// </summary>
    public sealed class PartElement
    {
        /// <summary>Тип: "p" | "br" | "title" | "subtitle" | "note".</summary>
        [JsonPropertyName("t")]
        public required string T { get; init; }

        /// <summary>Координата [bodyIdx, sectionIdx, elemIdx].</summary>
        [JsonPropertyName("xp")]
        public required int[] Xp { get; init; }

        /// <summary>Текстовое содержимое (отсутствует у "br").</summary>
        [JsonPropertyName("c")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? C { get; init; }

        /// <summary>
        /// Встроенные сноски: ключ = номер/идентификатор сноски, значение = текст.
        /// Присутствует только у "p" с хотя бы одной ссылкой на note.
        /// </summary>
        [JsonPropertyName("n")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? N { get; init; }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Промежуточные структуры парсера
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>Элемент, накапливаемый в процессе обхода XML до разбивки на фрагменты.</summary>
   public sealed class ParsedElement
    {
        public required string Type { get; init; }      // p / br / title / subtitle
        public string? Text { get; init; }              // плоский текст абзаца
        public Dictionary<string, string>? Notes { get; init; } // встроенные сноски

        // Координата, которая будет заполнена после прохода
        public int BodyIndex { get; set; }
        public int SectionIndex { get; set; }
        public int ElemIndex { get; set; }

        // Глобальный порядковый номер (s в Parts)
        public int GlobalIndex { get; set; }
    }

}
