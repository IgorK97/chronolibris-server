using System.Text.Json;
using System.Text.Json.Serialization;
using Chronolibris.Domain.Models;

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
    /// Один элемент JSON-фрагмента верхнего уровня.
    ///
    /// Поле c сериализуется по правилу:
    ///   • нет сносок  → просто строка:           "c": "текст абзаца"
    ///   • есть сноски → массив строк и Note:     "c": ["текст", {t:"note",...}, "продолжение"]
    ///   • br          → поле c отсутствует
    /// </summary>
    [JsonConverter(typeof(PartElementJsonConverter))] //Что такое typeof и что он здесь делает?
    public sealed class PartElement
    {
        public required string T { get; init; }
        public required int[] Xp { get; init; }

        /// <summary>
        /// Null → br (нет поля c).
        /// string → абзац без сносок (c = строка).
        /// List&lt;object&gt; → абзац со сносками (c = массив строк и NoteSegment).
        /// </summary>
        public object? C { get; init; }
    }

    /// <summary>
    /// Note-сегмент внутри смешанного массива c.
    /// Сериализуется как обычный JSON-объект {t,role,xp,c,f}.
    /// Текстовые части массива — просто string.
    /// </summary>
    public sealed class NoteSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "note";

        [JsonPropertyName("role")]
        public string Role { get; init; } = "footnote";

        /// <summary>xp сноски в notes-body.</summary>
        [JsonPropertyName("xp")]
        public required int[] Xp { get; init; }

        /// <summary>Видимая метка, например "[4]".</summary>
        [JsonPropertyName("c")]
        public required string C { get; init; }

        [JsonPropertyName("f")]
        public required FootnoteContent F { get; init; }
    }

    /// <summary>
    /// Раскрытое содержимое сноски.
    /// c — список параграфов; каждый параграф — строка (простой текст сноски).
    /// </summary>
    public sealed class FootnoteContent
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "footnote";

        [JsonPropertyName("xp")]
        public required int[] Xp { get; init; }

        /// <summary>
        /// Параграфы сноски. Каждый параграф — строка.
        /// (Если понадобится смешанный контент внутри сносок — расширить до List&lt;object&gt;.)
        /// </summary>
        [JsonPropertyName("c")]
        public required List<string> C { get; init; }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Кастомный сериализатор для PartElement
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Сериализует PartElement так, чтобы поле c было:
    ///   • строкой  — если C имеет тип string
    ///   • массивом — если C имеет тип List&lt;object&gt; (строки + NoteSegment)
    ///   • отсутствовало — если C == null
    /// </summary>
    public sealed class PartElementJsonConverter : JsonConverter<PartElement>
    {
        public override PartElement Read(ref Utf8JsonReader reader,
            Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException("Десериализация PartElement не реализована.");

        public override void Write(Utf8JsonWriter writer,
            PartElement value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("t", value.T);
            writer.WritePropertyName("xp");
            JsonSerializer.Serialize(writer, value.Xp, options);

            switch (value.C)
            {
                case null:
                    // br — поле c не пишем
                    break;

                case string plainText:
                    // Абзац без сносок → "c": "строка"
                    writer.WriteString("c", plainText);
                    break;

                case int pageNum:
                    // Номер страницы → "c": 10
                    writer.WriteNumber("c", pageNum);
                    break;

                case List<object> mixed:
                    // Абзац со сносками → "c": ["текст", {note}, "текст", ...]
                    writer.WritePropertyName("c");
                    writer.WriteStartArray();
                    foreach (var item in mixed)
                    {
                        if (item is string s)
                            writer.WriteStringValue(s);
                        else
                            JsonSerializer.Serialize(writer, item, item.GetType(), options);
                    }
                    writer.WriteEndArray();
                    break;
            }

            writer.WriteEndObject();
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Промежуточные структуры парсера
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>Элемент, накапливаемый в процессе обхода XML до разбивки на фрагменты.</summary>
    public sealed class ParsedElement
    {
        public required string Type { get; init; }   // p / br / title / subtitle

        /// <summary>
        /// null        → br
        /// string      → абзац без сносок
        /// List&lt;object&gt; → абзац со сносками (строки + NoteSegment)
        /// </summary>
        public object? Content { get; init; }

        // Координата
        public int BodyIndex { get; set; }
        public int SectionIndex { get; set; }
        public int ElemIndex { get; set; }

        // Глобальный порядковый номер (s в Parts)
        public int GlobalIndex { get; set; }

        // Плоский текст — только для BuildChapters / CalculateFullLength
        public string? Text { get; init; }
    }

    /// <summary>
    /// Сегмент курсивного текста внутри абзаца: {t:"em", c:"текст"}.
    /// </summary>
    public sealed class EmSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "em";

        [JsonPropertyName("c")]
        public required string C { get; init; }
    }

    /// <summary>
    /// Сегмент жирного текста внутри абзаца: {t:"st", c:"текст"}.
    /// </summary>
    public sealed class StSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "st";

        [JsonPropertyName("c")]
        public required string C { get; init; }
    }


    /// <summary>
    /// Сегмент изображения. Используется как элемент верхнего уровня (t="p" с единственным img)
    /// или как вложенный сегмент внутри абзаца.
    /// {t:"img", src:"1.jpg", w:768, h:230}
    /// w/h — null если размер не удалось определить.
    /// </summary>
    public sealed class ImgSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "img";

        /// <summary>Имя файла в MinIO: "1.jpg", "2.png" и т.д.</summary>
        [JsonPropertyName("src")]
        public required string Src { get; init; }
    }









    //// ─────────────────────────────────────────────────────────────────────────────
    //// Параметры конвертации
    //// ─────────────────────────────────────────────────────────────────────────────

    ///// <summary>
    ///// Настройки процесса конвертации.
    ///// </summary>
    //public sealed class ConversionOptions
    //{
    //    /// <summary>
    //    /// Целевое число элементов (абзацев, разрывов, заголовков) в одном фрагменте.
    //    /// По умолчанию 88 — как в образце.
    //    /// </summary>
    //    public int TargetPartSize { get; init; } = 88;

    //    /// <summary>
    //    /// Версия формата toc.json.
    //    /// </summary>
    //    public string FormatVersion { get; init; } = "1.1";
    //}

    //// ─────────────────────────────────────────────────────────────────────────────
    //// Результат конвертации (возвращается вызывающей стороне для записи в БД)
    //// ─────────────────────────────────────────────────────────────────────────────

    ///// <summary>
    ///// Итог конвертации одного FB2-файла.
    ///// </summary>
    //public sealed class ConversionResult
    //{
    //    /// <summary>UUID книги (из document-info или сгенерированный).</summary>
    //    public required string BookId { get; init; }

    //    /// <summary>Метаданные книги, извлечённые из FB2.</summary>
    //    public required BookMeta Meta { get; init; }

    //    /// <summary>Суммарное число элементов во всём теле книги.</summary>
    //    public int TotalElements { get; init; }

    //    /// <summary>Описание файла toc.json, сохранённого в хранилище.</summary>
    //    public required StoredFileInfo TocFile { get; init; }

    //    /// <summary>Описания фрагментных файлов (000.js, 001.js …).</summary>
    //    public required IReadOnlyList<StoredFileInfo> PartFiles { get; init; }

    //    /// <summary>Дата/время завершения конвертации (UTC).</summary>
    //    public DateTimeOffset CompletedAt { get; init; } = DateTimeOffset.UtcNow;
    //}

    ///// <summary>
    ///// Информация об одном файле, сохранённом в хранилище.
    ///// Достаточна для записи строки в таблицу book_files / book_parts.
    ///// </summary>
    //public sealed class StoredFileInfo
    //{
    //    /// <summary>UUID книги-владельца.</summary>
    //    public required string BookId { get; init; }

    //    /// <summary>Имя файла («toc.json», «000.js», «001.js» …).</summary>
    //    public required string FileName { get; init; }

    //    /// <summary>Тип файла.</summary>
    //    public required StoredFileType FileType { get; init; }

    //    /// <summary>Глобальный индекс первого элемента фрагмента (включительно). Для toc — 0.</summary>
    //    public int GlobalStart { get; init; }

    //    /// <summary>Глобальный индекс последнего элемента фрагмента (включительно). Для toc — 0.</summary>
    //    public int GlobalEnd { get; init; }

    //    /// <summary>Координата первого элемента [bodyIdx, sectionIdx, elemIdx]. Для toc — null.</summary>
    //    public int[]? XpStart { get; init; }

    //    /// <summary>Координата последнего элемента. Для toc — null.</summary>
    //    public int[]? XpEnd { get; init; }

    //    /// <summary>Размер содержимого в байтах (UTF-8).</summary>
    //    public long SizeBytes { get; init; }
    //}

    //public enum StoredFileType
    //{
    //    Toc,
    //    Part
    //}

    //// ─────────────────────────────────────────────────────────────────────────────
    //// Метаданные книги
    //// ─────────────────────────────────────────────────────────────────────────────

    //public sealed record BookMeta
    //{
    //    [JsonPropertyName("Annotation")]
    //    public string? Annotation { get; init; }

    //    [JsonPropertyName("Lang")]
    //    public string? Lang { get; init; }

    //    [JsonPropertyName("Title")]
    //    public string? Title { get; init; }

    //    [JsonPropertyName("Sequences")]
    //    public List<string>? Sequences { get; init; }

    //    [JsonPropertyName("Created")]
    //    public string? Created { get; init; }

    //    [JsonPropertyName("Updated")]
    //    public string? Updated { get; init; }

    //    [JsonPropertyName("Written")]
    //    public WrittenInfo? Written { get; init; }

    //    [JsonPropertyName("Authors")]
    //    public List<AuthorInfo>? Authors { get; init; }

    //    [JsonPropertyName("ArtID")]
    //    public string? ArtId { get; init; }

    //    [JsonPropertyName("UUID")]
    //    public string? Uuid { get; init; }

    //    [JsonPropertyName("version")]
    //    public string? Version { get; init; }
    //}

    //public sealed record WrittenInfo
    //{
    //    [JsonPropertyName("DatePublic")]
    //    public string? DatePublic { get; init; }

    //    [JsonPropertyName("Date")]
    //    public string? Date { get; init; }
    //}

    //public sealed record AuthorInfo
    //{
    //    [JsonPropertyName("Role")]
    //    public string Role { get; init; } = "author";

    //    [JsonPropertyName("First")]
    //    public string? First { get; init; }

    //    [JsonPropertyName("Last")]
    //    public string? Last { get; init; }
    //}

    //// ─────────────────────────────────────────────────────────────────────────────
    //// Внутренние модели toc.json
    //// ─────────────────────────────────────────────────────────────────────────────

    ///// <summary>Корень toc.json.</summary>
    //public sealed class TocDocument
    //{
    //    [JsonPropertyName("Meta")]
    //    public required BookMeta Meta { get; init; }

    //    [JsonPropertyName("full_length")]
    //    public int FullLength { get; set; }

    //    [JsonPropertyName("Body")]
    //    public required List<TocBodyEntry> Body { get; init; }

    //    [JsonPropertyName("Parts")]
    //    public required List<TocPartEntry> Parts { get; init; }
    //}

    ///// <summary>Запись верхнего уровня Body: описывает всю книгу или её том.</summary>
    //public sealed class TocBodyEntry
    //{
    //    [JsonPropertyName("s")]
    //    public int S { get; set; }

    //    [JsonPropertyName("e")]
    //    public int E { get; set; }

    //    [JsonPropertyName("t")]
    //    public string? T { get; set; }

    //    [JsonPropertyName("c")]
    //    public List<TocChapterEntry>? C { get; set; }
    //}

    ///// <summary>Запись одной главы / раздела в Body.</summary>
    //public sealed class TocChapterEntry
    //{
    //    [JsonPropertyName("s")]
    //    public int S { get; set; }

    //    [JsonPropertyName("e")]
    //    public int E { get; set; }

    //    [JsonPropertyName("t")]
    //    public string? T { get; set; }
    //}

    ///// <summary>Запись одного фрагментного файла в Parts.</summary>
    //public sealed class TocPartEntry
    //{
    //    [JsonPropertyName("s")]
    //    public int S { get; set; }

    //    [JsonPropertyName("e")]
    //    public int E { get; set; }

    //    [JsonPropertyName("xps")]
    //    public required int[] Xps { get; set; }

    //    [JsonPropertyName("xpe")]
    //    public required int[] Xpe { get; set; }

    //    [JsonPropertyName("url")]
    //    public required string Url { get; set; }
    //}

    //// ─────────────────────────────────────────────────────────────────────────────
    //// Элементы фрагмента (*.js)
    //// ─────────────────────────────────────────────────────────────────────────────

    ///// <summary>
    ///// Один элемент JSON-фрагмента верхнего уровня.
    ///// t = "p" | "br" | "title" | "subtitle"
    /////
    ///// Для "p" и "title" поле c содержит массив TextSegment —
    ///// смешанный контент: строки и Note-объекты.
    ///// Для "br" поле c отсутствует.
    ///// </summary>
    //public sealed class PartElement
    //{
    //    [JsonPropertyName("t")]
    //    public required string T { get; init; }

    //    [JsonPropertyName("xp")]
    //    public required int[] Xp { get; init; }

    //    /// <summary>
    //    /// Для "p" / "title" / "subtitle": массив TextSegment (строки + Note).
    //    /// Для "br": null (поле не выводится).
    //    /// Сериализуется как JSON-массив.
    //    /// </summary>
    //    [JsonPropertyName("c")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public List<TextSegment>? C { get; init; }
    //}

    ///// <summary>
    ///// Один сегмент смешанного содержимого абзаца.
    ///// t = "text"  → c содержит строку
    ///// t = "note"  → объект-ссылка на сноску, c = видимая метка "[4]", f = раскрытая сноска
    ///// </summary>
    //public sealed class TextSegment
    //{
    //    [JsonPropertyName("t")]
    //    public required string T { get; init; }

    //    /// <summary>
    //    /// Для t="text": строка с текстом.
    //    /// Для t="note": видимая метка, например "[4]".
    //    /// </summary>
    //    [JsonPropertyName("c")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public string? C { get; init; }

    //    /// <summary>Только для t="note". Роль: всегда "footnote".</summary>
    //    [JsonPropertyName("role")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public string? Role { get; init; }

    //    /// <summary>
    //    /// Только для t="note". xp сноски в notes-body: [bodyIdx, sectionIdx, elemIdx].
    //    /// bodyIdx для notes-body всегда 2 (второй body в документе).
    //    /// </summary>
    //    [JsonPropertyName("xp")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public int[]? Xp { get; init; }

    //    /// <summary>Только для t="note". Раскрытое содержимое сноски.</summary>
    //    [JsonPropertyName("f")]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public FootnoteContent? F { get; init; }
    //}

    ///// <summary>
    ///// Раскрытое содержимое сноски (поле f внутри Note).
    ///// t = "footnote"
    ///// c = массив параграфов сноски (каждый — List&lt;TextSegment&gt; для единообразия)
    ///// </summary>
    //public sealed class FootnoteContent
    //{
    //    [JsonPropertyName("t")]
    //    public string T { get; init; } = "footnote";

    //    [JsonPropertyName("xp")]
    //    public required int[] Xp { get; init; }

    //    /// <summary>Параграфы сноски. Каждый элемент — массив TextSegment.</summary>
    //    [JsonPropertyName("c")]
    //    public required List<List<TextSegment>> C { get; init; }
    //}

    //// ─────────────────────────────────────────────────────────────────────────────
    //// Промежуточные структуры парсера
    //// ─────────────────────────────────────────────────────────────────────────────

    ///// <summary>Разобранная ссылка на сноску внутри абзаца.</summary>
    //public sealed class ParsedNoteRef
    //{
    //    /// <summary>Оригинальный id из href="#nX" → "nX".</summary>
    //    public required string NoteId { get; init; }

    //    /// <summary>Видимая метка в тексте, например "[4]".</summary>
    //    public required string Label { get; init; }

    //    /// <summary>xp сноски в notes-body.</summary>
    //    public required int[] Xp { get; init; }

    //    /// <summary>Разобранные параграфы сноски.</summary>
    //    public required List<List<TextSegment>> Paragraphs { get; init; }
    //}

    ///// <summary>Элемент, накапливаемый в процессе обхода XML до разбивки на фрагменты.</summary>
    //public sealed class ParsedElement
    //{
    //    public required string Type { get; init; }      // p / br / title / subtitle

    //    /// <summary>
    //    /// Смешанный контент абзаца: List&lt;TextSegment&gt; (строки + Note-объекты).
    //    /// Для br — null.
    //    /// </summary>
    //    public List<TextSegment>? Segments { get; init; }

    //    // Координата
    //    public int BodyIndex { get; set; }
    //    public int SectionIndex { get; set; }
    //    public int ElemIndex { get; set; }

    //    // Глобальный порядковый номер (s в Parts)
    //    public int GlobalIndex { get; set; }

    //    // Плоский текст — используется только для BuildChapters / CalculateFullLength
    //    public string? Text { get; init; }
    //}













    //     // ─────────────────────────────────────────────────────────────────────────────
    // // Параметры конвертации
    // // ─────────────────────────────────────────────────────────────────────────────

    // /// <summary>
    // /// Настройки процесса конвертации.
    // /// </summary>
    // public sealed class ConversionOptions
    // {
    //     /// <summary>
    //     /// Целевое число элементов (абзацев, разрывов, заголовков) в одном фрагменте.
    //     /// По умолчанию 88 — как в образце.
    //     /// </summary>
    //     public int TargetPartSize { get; init; } = 88;

    //     /// <summary>
    //     /// Версия формата toc.json.
    //     /// </summary>
    //     public string FormatVersion { get; init; } = "1.1";
    // }

    // // ─────────────────────────────────────────────────────────────────────────────
    // // Результат конвертации (возвращается вызывающей стороне для записи в БД)
    // // ─────────────────────────────────────────────────────────────────────────────

    // /// <summary>
    // /// Итог конвертации одного FB2-файла.
    // /// </summary>
    // public sealed class ConversionResult
    // {
    //     /// <summary>UUID книги (из document-info или сгенерированный).</summary>
    //     public required string BookId { get; init; }

    //     /// <summary>Метаданные книги, извлечённые из FB2.</summary>
    //     public required BookMeta Meta { get; init; }

    //     /// <summary>Суммарное число элементов во всём теле книги.</summary>
    //     public int TotalElements { get; init; }

    //     /// <summary>Описание файла toc.json, сохранённого в хранилище.</summary>
    //     public required StoredFileInfo TocFile { get; init; }

    //     /// <summary>Описания фрагментных файлов (000.js, 001.js …).</summary>
    //     public required IReadOnlyList<StoredFileInfo> PartFiles { get; init; }

    //     /// <summary>Дата/время завершения конвертации (UTC).</summary>
    //     public DateTimeOffset CompletedAt { get; init; } = DateTimeOffset.UtcNow;
    // }

    // /// <summary>
    // /// Информация об одном файле, сохранённом в хранилище.
    // /// Достаточна для записи строки в таблицу book_files / book_parts.
    // /// </summary>
    // public sealed class StoredFileInfo
    // {
    //     /// <summary>UUID книги-владельца.</summary>
    //     public required string BookId { get; init; }

    //     /// <summary>Имя файла («toc.json», «000.js», «001.js» …).</summary>
    //     public required string FileName { get; init; }

    //     /// <summary>Тип файла.</summary>
    //     public required StoredFileType FileType { get; init; }

    //     /// <summary>Глобальный индекс первого элемента фрагмента (включительно). Для toc — 0.</summary>
    //     public int GlobalStart { get; init; }

    //     /// <summary>Глобальный индекс последнего элемента фрагмента (включительно). Для toc — 0.</summary>
    //     public int GlobalEnd { get; init; }

    //     /// <summary>Координата первого элемента [bodyIdx, sectionIdx, elemIdx]. Для toc — null.</summary>
    //     public int[]? XpStart { get; init; }

    //     /// <summary>Координата последнего элемента. Для toc — null.</summary>
    //     public int[]? XpEnd { get; init; }

    //     /// <summary>Размер содержимого в байтах (UTF-8).</summary>
    //     public long SizeBytes { get; init; }
    // }

    // public enum StoredFileType
    // {
    //     Toc,
    //     Part
    // }

    // // ─────────────────────────────────────────────────────────────────────────────
    // // Метаданные книги
    // // ─────────────────────────────────────────────────────────────────────────────

    // public sealed record BookMeta
    // {
    //     [JsonPropertyName("Annotation")]
    //     public string? Annotation { get; init; }

    //     [JsonPropertyName("Lang")]
    //     public string? Lang { get; init; }

    //     [JsonPropertyName("Title")]
    //     public string? Title { get; init; }

    //     [JsonPropertyName("Sequences")]
    //     public List<string>? Sequences { get; init; }

    //     [JsonPropertyName("Created")]
    //     public string? Created { get; init; }

    //     [JsonPropertyName("Updated")]
    //     public string? Updated { get; init; }

    //     [JsonPropertyName("Written")]
    //     public WrittenInfo? Written { get; init; }

    //     [JsonPropertyName("Authors")]
    //     public List<AuthorInfo>? Authors { get; init; }

    //     [JsonPropertyName("ArtID")]
    //     public string? ArtId { get; init; }

    //     [JsonPropertyName("UUID")]
    //     public string? Uuid { get; init; }

    //     [JsonPropertyName("version")]
    //     public string? Version { get; init; }
    // }

    // public sealed record WrittenInfo
    // {
    //     [JsonPropertyName("DatePublic")]
    //     public string? DatePublic { get; init; }

    //     [JsonPropertyName("Date")]
    //     public string? Date { get; init; }
    // }

    // public sealed record AuthorInfo
    // {
    //     [JsonPropertyName("Role")]
    //     public string Role { get; init; } = "author";

    //     [JsonPropertyName("First")]
    //     public string? First { get; init; }

    //     [JsonPropertyName("Last")]
    //     public string? Last { get; init; }
    // }

    // // ─────────────────────────────────────────────────────────────────────────────
    // // Внутренние модели toc.json
    // // ─────────────────────────────────────────────────────────────────────────────

    // /// <summary>Корень toc.json.</summary>
    // public sealed class TocDocument
    // {
    //     [JsonPropertyName("Meta")]
    //     public required BookMeta Meta { get; init; }

    //     [JsonPropertyName("full_length")]
    //     public int FullLength { get; set; }

    //     [JsonPropertyName("Body")]
    //     public required List<TocBodyEntry> Body { get; init; }

    //     [JsonPropertyName("Parts")]
    //     public required List<TocPartEntry> Parts { get; init; }
    // }

    // /// <summary>Запись верхнего уровня Body: описывает всю книгу или её том.</summary>
    // public sealed class TocBodyEntry
    // {
    //     [JsonPropertyName("s")]
    //     public int S { get; set; }

    //     [JsonPropertyName("e")]
    //     public int E { get; set; }

    //     [JsonPropertyName("t")]
    //     public string? T { get; set; }

    //     [JsonPropertyName("c")]
    //     public List<TocChapterEntry>? C { get; set; }
    // }

    // /// <summary>Запись одной главы / раздела в Body.</summary>
    // public sealed class TocChapterEntry
    // {
    //     [JsonPropertyName("s")]
    //     public int S { get; set; }

    //     [JsonPropertyName("e")]
    //     public int E { get; set; }

    //     [JsonPropertyName("t")]
    //     public string? T { get; set; }
    // }

    // /// <summary>Запись одного фрагментного файла в Parts.</summary>
    // public sealed class TocPartEntry
    // {
    //     [JsonPropertyName("s")]
    //     public int S { get; set; }

    //     [JsonPropertyName("e")]
    //     public int E { get; set; }

    //     [JsonPropertyName("xps")]
    //     public required int[] Xps { get; set; }

    //     [JsonPropertyName("xpe")]
    //     public required int[] Xpe { get; set; }

    //     [JsonPropertyName("url")]
    //     public required string Url { get; set; }
    // }

    // // ─────────────────────────────────────────────────────────────────────────────
    // // Элементы фрагмента (*.js)
    // // ─────────────────────────────────────────────────────────────────────────────

    // /// <summary>
    // /// Один элемент JSON-фрагмента.
    // /// </summary>
    // public sealed class PartElement
    // {
    //     /// <summary>Тип: "p" | "br" | "title" | "subtitle" | "note".</summary>
    //     [JsonPropertyName("t")]
    //     public required string T { get; init; }

    //     /// <summary>Координата [bodyIdx, sectionIdx, elemIdx].</summary>
    //     [JsonPropertyName("xp")]
    //     public required int[] Xp { get; init; }

    //     /// <summary>Текстовое содержимое (отсутствует у "br").</summary>
    //     [JsonPropertyName("c")]
    //     [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //     public string? C { get; init; }

    //     /// <summary>
    //     /// Встроенные сноски: ключ = номер/идентификатор сноски, значение = текст.
    //     /// Присутствует только у "p" с хотя бы одной ссылкой на note.
    //     /// </summary>
    //     [JsonPropertyName("n")]
    //     [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //     public Dictionary<string, string>? N { get; init; }
    // }

    // // ─────────────────────────────────────────────────────────────────────────────
    // // Промежуточные структуры парсера
    // // ─────────────────────────────────────────────────────────────────────────────

    // /// <summary>Элемент, накапливаемый в процессе обхода XML до разбивки на фрагменты.</summary>
    //public sealed class ParsedElement
    // {
    //     public required string Type { get; init; }      // p / br / title / subtitle
    //     public string? Text { get; init; }              // плоский текст абзаца
    //     public Dictionary<string, string>? Notes { get; init; } // встроенные сноски

    //     // Координата, которая будет заполнена после прохода
    //     public int BodyIndex { get; set; }
    //     public int SectionIndex { get; set; }
    //     public int ElemIndex { get; set; }

    //     // Глобальный порядковый номер (s в Parts)
    //     public int GlobalIndex { get; set; }
    // }

}
