//using System.Text.Json;
//using System.Text.Json.Serialization;
//using Chronolibris.Domain.Models;

//namespace Chronolibris.Application.Interfaces
//{
//    // Настройки процесса конвертации
//    public class ConversionOptions
//    {
//        // Целевое число элементов (абзацев, разрывов, заголовков) в одном фрагменте
//        public int TargetPartSize { get; init; } = 100;
//    }

//    //Корень для toc.json
//    public class TocDocument
//    {
//        [JsonPropertyName("Meta")]
//        public required BookMeta Meta { get; init; }

//        [JsonPropertyName("full_length")]
//        public int FullLength { get; set; }

//        [JsonPropertyName("Body")]
//        public required List<TocBody> Body { get; init; }

//        [JsonPropertyName("Parts")]
//        public required List<TocPart> Parts { get; init; }
//    }

//    //Запись верхнего уровня Body: описывает всю книгу
//    public class TocBody
//    {
//        [JsonPropertyName("s")]
//        public int S { get; set; } //стартовая позиция

//        [JsonPropertyName("e")]
//        public int E { get; set; } //конечная позиция

//        [JsonPropertyName("t")]
//        public string? T { get; set; } //тип тега

//        [JsonPropertyName("c")]
//        public List<TocChapter>? C { get; set; } //содержимое (список глав)
//    }

//    //Глава (как часть содержания)
//    public class TocChapter
//    {
//        [JsonPropertyName("s")]
//        public int S { get; set; }

//        [JsonPropertyName("e")]
//        public int E { get; set; }

//        [JsonPropertyName("t")]
//        public string? T { get; set; }
//    }

//    //Информация о фрагменте книги
//    public class TocPart
//    {
//        [JsonPropertyName("s")]
//        public int S { get; set; }

//        [JsonPropertyName("e")]
//        public int E { get; set; }

//        [JsonPropertyName("xps")]
//        public required int[] Xps { get; set; }

//        [JsonPropertyName("xpe")]
//        public required int[] Xpe { get; set; }

//        [JsonPropertyName("url")]
//        public required string Url { get; set; }
//    }

//    /// <summary>
//    /// Один элемент JSON-фрагмента верхнего уровня
//    /// Если нет сносок, то это просто строка ("c" : "...")
//    /// Если есть сноски, то массив строк и Note ( "c" : ["текст перед сноской...", {t:"note", ...}, "...текст после сноски"]
//    /// Поле может отсутствовать, если тип тега br, или быть числом (номер страницы)
//    /// </summary>
//    [JsonConverter(typeof(PartElementJsonConverter))] //Что такое typeof и что он здесь делает? И зачем нужно определять свой конвертер?
//    public class PartElement
//    {
//        public required string T { get; init; }
//        public required int[] Xp { get; init; }
//        public object? C { get; init; }
//    }

//    //Заметка (сноска)
//    public class NoteSegment
//    {
//        [JsonPropertyName("t")]
//        public string T { get; init; } = "note";

//        [JsonPropertyName("role")]
//        public string Role { get; init; } = "footnote";

//        //xp сноски в notes-body
//        [JsonPropertyName("xp")]
//        public required int[] Xp { get; init; }

//        //Видимая метка, например "[4]"
//        [JsonPropertyName("c")]
//        public required string C { get; init; }

//        [JsonPropertyName("f")]
//        public required FootnoteContent F { get; init; }
//    }

//    /// <summary>
//    /// Раскрытое содержимое сноски
//    /// c — список параграфов; каждый параграф — строка (простой текст сноски)
//    /// </summary>
//    public class FootnoteContent
//    {
//        [JsonPropertyName("t")]
//        public string T { get; init; } = "footnote";

//        [JsonPropertyName("xp")]
//        public required int[] Xp { get; init; }

//        // Параграфы сноски. Каждый параграф — строка.
//        [JsonPropertyName("c")]
//        public required List<string> C { get; init; }
//    }


//    /// <summary>
//    /// Сериализует PartElement так, чтобы поле c было либо строкой, либо массивом, либо отсутствовало
//    /// </summary>
//    public class PartElementJsonConverter : JsonConverter<PartElement>
//    {
//        //Можно ли было обойтись только методом сериализации?
//        public override PartElement Read(ref Utf8JsonReader reader,
//            Type typeToConvert, JsonSerializerOptions options)
//            => throw new NotSupportedException("Десериализация на данный момент не определена"); //мне сейчас нужна только сериализация, десериализуется все на клиенте

//        public override void Write(Utf8JsonWriter writer,
//            PartElement value, JsonSerializerOptions options)
//        {
//            writer.WriteStartObject();
//            writer.WriteString("t", value.T);
//            writer.WritePropertyName("xp");
//            JsonSerializer.Serialize(writer, value.Xp, options);

//            switch (value.C)
//            {
//                case null:
//                    // br — поле писать не нужно
//                    break;

//                case string plainText:
//                    // если это чистая строка, то нужно записать
//                    writer.WriteString("c", plainText);
//                    break;

//                case int pageNum:
//                    // Номер страницы
//                    writer.WriteNumber("c", pageNum);
//                    break;

//                case List<object> mixed:
//                    // Абзац со сносками (или, как вариант, с одной сноской)
//                    writer.WritePropertyName("c");
//                    writer.WriteStartArray();
//                    foreach (var item in mixed)
//                    {
//                        if (item is string s)
//                            writer.WriteStringValue(s);
//                        else
//                            JsonSerializer.Serialize(writer, item, item.GetType(), options);
//                    }
//                    writer.WriteEndArray();
//                    break;
//            }

//            writer.WriteEndObject();
//        }
//    }


//    //Элемент, накапливаемый в процессе обхода XML до разбивки на фрагменты
//    public class ParsedElement
//    {
//        public required string Type { get; init; }   // p / br / title / subtitle - тип тега
//        public object? Content { get; init; } //Само содержимое
//        public int BodyIndex { get; set; } //позиция (всего три координаты, но если структура файлов этого потребует, можно увеличить, сделав массив или что-то вроде того)
//        public int SectionIndex { get; set; } 
//        public int ElemIndex { get; set; }
//        public int GlobalIndex { get; set; } //глобальный порядковый номер
//        public string? Text { get; init; } //чистый текст
//    }


//    //Курсивный текст внутри абзаца {t:"em", c:"текст"}.
//    public sealed class EmSegment
//    {
//        [JsonPropertyName("t")]
//        public string T { get; init; } = "em";

//        [JsonPropertyName("c")]
//        public required string C { get; init; }
//    }

//    // Жирный текст внутри абзаца (st - strong, в файл тоже будет записываться кратко для экономии места) {t:"st", c:"текст"}.
//    public sealed class StSegment
//    {
//        [JsonPropertyName("t")]
//        public string T { get; init; } = "st";

//        [JsonPropertyName("c")]
//        public required string C { get; init; }
//    }


//    /// <summary>
//    /// Сегмент изображения. Используется как элемент верхнего уровня (t="p" с единственным img)
//    /// или как вложенный сегмент внутри абзаца
//    /// {t:"img", src:"1.jpg"}
//    /// </summary>
//    public sealed class ImgSegment
//    {
//        [JsonPropertyName("t")]
//        public string T { get; init; } = "img";

//        [JsonPropertyName("src")]
//        public required string Src { get; init; }
//    }


//    public sealed class ParsedNote
//    {
//        public required string NoteId { get; init; }
//        public required int[] Xp { get; init; } //с 1 отсчет
//        public required List<string> Paragraphs { get; init; }
//    }

//    public class PageNumberSegment
//    {
//        [JsonPropertyName("pn")]
//        public int Pn { get; set; }
//    }
//}


using System.Text.Json;
using System.Text.Json.Serialization;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Interfaces
{
    // Настройки процесса конвертации
    public class ConversionOptions
    {
        // Целевое число элементов (абзацев, разрывов, заголовков) в одном фрагменте
        public int TargetPartSize { get; init; } = 100;
    }

    //Корень для toc.json
    public class TocDocument
    {
        [JsonPropertyName("Meta")]
        public required BookMeta Meta { get; init; }

        [JsonPropertyName("full_length")]
        public int FullLength { get; set; }

        [JsonPropertyName("Body")]
        public required List<TocBody> Body { get; init; }

        [JsonPropertyName("Parts")]
        public required List<TocPart> Parts { get; init; }
    }

    //Запись верхнего уровня Body: описывает всю книгу
    public class TocBody
    {
        [JsonPropertyName("s")]
        public int S { get; set; } //стартовая позиция

        [JsonPropertyName("e")]
        public int E { get; set; } //конечная позиция

        [JsonPropertyName("t")]
        public string? T { get; set; } //тип тега

        [JsonPropertyName("c")]
        public List<TocChapter>? C { get; set; } //содержимое (список глав)
    }

    //Глава (как часть содержания)
    public class TocChapter
    {
        [JsonPropertyName("s")]
        public int S { get; set; }

        [JsonPropertyName("e")]
        public int E { get; set; }

        [JsonPropertyName("t")]
        public string? T { get; set; }
        [JsonPropertyName("c")]
        public List<TocChapter> C { get; set; } = new();
    }

    //Информация о фрагменте книги
    public class TocPart
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

    /// <summary>
    /// Один элемент JSON-фрагмента верхнего уровня
    /// Если нет сносок, то это просто строка ("c" : "...")
    /// Если есть сноски, то массив строк и Note ( "c" : ["текст перед сноской...", {t:"note", ...}, "...текст после сноски"]
    /// Поле может отсутствовать, если тип тега br, или быть числом (номер страницы)
    /// </summary>
    [JsonConverter(typeof(PartElementJsonConverter))] //Что такое typeof и что он здесь делает? И зачем нужно определять свой конвертер?
    public class PartElement
    {
        public required string T { get; init; }
        public required int[] Xp { get; init; }
        public object? C { get; init; }
    }

    //Заметка (сноска)
    public class NoteSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "note";

        [JsonPropertyName("role")]
        public string Role { get; init; } = "footnote";

        //xp сноски в notes-body
        [JsonPropertyName("xp")]
        public required int[] Xp { get; init; }

        //Видимая метка, например "[4]"
        [JsonPropertyName("c")]
        public required string C { get; init; }

        [JsonPropertyName("f")]
        public required FootnoteContent F { get; init; }
    }

    /// <summary>
    /// Раскрытое содержимое сноски
    /// c — список параграфов; каждый параграф — строка (простой текст сноски)
    /// </summary>
    public class FootnoteContent
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "footnote";

        [JsonPropertyName("xp")]
        public required int[] Xp { get; init; }

        // Параграфы сноски. Каждый параграф — строка.
        [JsonPropertyName("c")]
        public required List<string> C { get; init; }
    }


    /// <summary>
    /// Сериализует PartElement так, чтобы поле c было либо строкой, либо массивом, либо отсутствовало
    /// </summary>
    public class PartElementJsonConverter : JsonConverter<PartElement>
    {
        //Можно ли было обойтись только методом сериализации?
        public override PartElement Read(ref Utf8JsonReader reader,
            Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException("Десериализация на данный момент не определена"); //мне сейчас нужна только сериализация, десериализуется все на клиенте

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
                    // br — поле писать не нужно
                    break;

                case string plainText:
                    // если это чистая строка, то нужно записать
                    writer.WriteString("c", plainText);
                    break;

                case int pageNum:
                    // Номер страницы
                    writer.WriteNumber("c", pageNum);
                    break;

                case List<object> mixed:
                    // Абзац со сносками (или, как вариант, с одной сноской)
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


    //Элемент, накапливаемый в процессе обхода XML до разбивки на фрагменты
    public class ParsedElement
    {
        public required string Type { get; init; }   // p / br / title / subtitle - тип тега
        public object? Content { get; init; } //Само содержимое
        /// <summary>
        /// Координаты элемента в дереве документа.
        /// Xp[0] — индекс body, Xp[1..^1] — индексы вложенных секций (по одному на каждый уровень),
        /// Xp[^1] — порядковый номер элемента внутри непосредственно родительской секции.
        /// Длина массива = 1 (нет секций) + глубина вложенности секций + 1.
        /// </summary>
        public required int[] Xp { get; init; }
        public int GlobalIndex { get; set; } //глобальный порядковый номер
        public string? Text { get; init; } //чистый текст
    }


    //Курсивный текст внутри абзаца {t:"em", c:"текст"}.
    public sealed class EmSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "em";

        [JsonPropertyName("c")]
        public required string C { get; init; }
    }

    // Жирный текст внутри абзаца (st - strong, в файл тоже будет записываться кратко для экономии места) {t:"st", c:"текст"}.
    public sealed class StSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "st";

        [JsonPropertyName("c")]
        public required string C { get; init; }
    }


    /// <summary>
    /// Сегмент изображения. Используется как элемент верхнего уровня (t="p" с единственным img)
    /// или как вложенный сегмент внутри абзаца
    /// {t:"img", src:"1.jpg"}
    /// </summary>
    public sealed class ImgSegment
    {
        [JsonPropertyName("t")]
        public string T { get; init; } = "img";

        [JsonPropertyName("src")]
        public required string Src { get; init; }
    }


    public sealed class ParsedNote
    {
        public required string NoteId { get; init; }
        public required int[] Xp { get; init; } //с 1 отсчет
        public required List<string> Paragraphs { get; init; }
    }

    public class PageNumberSegment
    {
        [JsonPropertyName("pn")]
        public int Pn { get; set; }
    }
}