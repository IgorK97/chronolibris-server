//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chronolibris.Application.Fb2Converter.Interfaces;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Text.RegularExpressions;
//using System.Xml;
//using System.Xml.Linq;
//using Chronolibris.Application.Fb2Converter;
//using Microsoft.AspNetCore.WebUtilities;
//using Chronolibris.Domain.Models;

//namespace Chronolibris.Infrastructure.DataAccess.Fb2Converter
//{


//    public sealed class Fb2ConverterXDoc : IFb2Converter
//    {
//        private const string Fb2Ns = "http://www.gribuser.ru/xml/fictionbook/2.0";
//        private const string XlinkNs = "http://www.w3.org/1999/xlink";

//        private static readonly XName NsP = XName.Get("p", Fb2Ns);
//        private static readonly XName NsA = XName.Get("a", Fb2Ns);
//        private static readonly XName NsStrong = XName.Get("strong", Fb2Ns);
//        private static readonly XName NsEmphasis = XName.Get("emphasis", Fb2Ns);
//        private static readonly XName NsSection = XName.Get("section", Fb2Ns);
//        private static readonly XName NsTitle = XName.Get("title", Fb2Ns);
//        private static readonly XName NsSubtitle = XName.Get("subtitle", Fb2Ns);
//        private static readonly XName NsBody = XName.Get("body", Fb2Ns);
//        private static readonly XName NsDescription = XName.Get("description", Fb2Ns);
//        private static readonly XName NsAnnotation = XName.Get("annotation", Fb2Ns);
//        private static readonly XName NsEmptyLine = XName.Get("empty-line", Fb2Ns);

//        private static readonly JsonSerializerOptions JsonOpts = new()
//        {
//            WriteIndented = true,
//            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
//        };

//        private readonly IBookStorage _storage;

//        public Fb2ConverterXDoc(IBookStorage storage)
//        {
//            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // Public API
//        // ══════════════════════════════════════════════════════════════════════

//        public async Task<ConversionResult> ConvertAsync(
//            Stream fb2Stream,
//            string? bookId = null,
//            ConversionOptions? options = null,
//            CancellationToken cancellationToken = default)
//        {
//            options ??= new ConversionOptions();

//            var doc = await LoadXmlAsync(fb2Stream, cancellationToken);

//            var rawMeta = ExtractMeta(doc);
//            var resolvedBookId = bookId ?? rawMeta.Uuid ?? Guid.NewGuid().ToString("D");

//            var meta = rawMeta with
//            {
//                Uuid = resolvedBookId,
//                Version = options.FormatVersion
//            };

//            // Словарь сносок: noteId → ParsedNote (с xp и параграфами)
//            var notes = ExtractNotes(doc);

//            // Основной обход → список элементов
//            var elements = ParseMainBody(doc, notes);

//            // TOC
//            var (tocDoc, _) = BuildToc(meta, elements, options);

//            // toc.json
//            var tocJson = JsonSerializer.Serialize(tocDoc, JsonOpts);
//            var tocBytes = Encoding.UTF8.GetByteCount(tocJson);
//            await _storage.SaveAsync(resolvedBookId, "toc.json", tocJson, cancellationToken);

//            var tocFileInfo = new StoredFileInfo
//            {
//                BookId = resolvedBookId,
//                FileName = "toc.json",
//                FileType = StoredFileType.Toc,
//                SizeBytes = tocBytes
//            };

//            // Фрагменты
//            var partFileInfos = new List<StoredFileInfo>();

//            foreach (var part in tocDoc.Parts)
//            {
//                cancellationToken.ThrowIfCancellationRequested();

//                var partElements = elements
//                    .Where(e => e.GlobalIndex >= part.S && e.GlobalIndex <= part.E)
//                    .Select(MapToPartElement)
//                    .ToList();

//                var partJson = JsonSerializer.Serialize(partElements, JsonOpts);
//                var partBytes = Encoding.UTF8.GetByteCount(partJson);
//                await _storage.SaveAsync(resolvedBookId, part.Url, partJson, cancellationToken);

//                partFileInfos.Add(new StoredFileInfo
//                {
//                    BookId = resolvedBookId,
//                    FileName = part.Url,
//                    FileType = StoredFileType.Part,
//                    GlobalStart = part.S,
//                    GlobalEnd = part.E,
//                    XpStart = part.Xps,
//                    XpEnd = part.Xpe,
//                    SizeBytes = partBytes
//                });
//            }

//            return new ConversionResult
//            {
//                BookId = resolvedBookId,
//                Meta = meta,
//                TotalElements = elements.Count,
//                TocFile = tocFileInfo,
//                PartFiles = partFileInfos,
//                CompletedAt = DateTimeOffset.UtcNow
//            };
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // Загрузка XML
//        // ══════════════════════════════════════════════════════════════════════

//        private static async Task<XDocument> LoadXmlAsync(Stream stream, CancellationToken ct)
//        {
//            using var ms = new MemoryStream();
//            await stream.CopyToAsync(ms, ct);
//            ms.Position = 0;

//            var settings = new XmlReaderSettings
//            {
//                DtdProcessing = DtdProcessing.Ignore,
//                IgnoreWhitespace = false,
//                Async = true
//            };
//            using var reader = XmlReader.Create(ms, settings);
//            return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // Метаданные
//        // ══════════════════════════════════════════════════════════════════════

//        private static BookMeta ExtractMeta(XDocument doc)
//        {
//            var desc = doc.Root?.Element(NsDescription);
//            var ti = desc?.Element(XName.Get("title-info", Fb2Ns));
//            var di = desc?.Element(XName.Get("document-info", Fb2Ns));

//            var authors = ti?.Elements(XName.Get("author", Fb2Ns))
//                .Select(a => new AuthorInfo
//                {
//                    Role = "author",
//                    First = a.Element(XName.Get("first-name", Fb2Ns))?.Value.Trim(),
//                    Last = a.Element(XName.Get("last-name", Fb2Ns))?.Value.Trim()
//                }).ToList();

//            var sequences = ti?.Elements(XName.Get("sequence", Fb2Ns))
//                .Select(s => s.Attribute("name")?.Value)
//                .Where(n => n != null).Cast<string>().ToList();

//            var annotation = ti?.Element(NsAnnotation) is XElement ann
//                ? string.Join(" ", ann.Descendants(NsP).Select(FlattenText)).Trim()
//                : null;

//            var publishInfo = desc?.Element(XName.Get("publish-info", Fb2Ns));
//            var yearPublic = publishInfo?.Element(XName.Get("year", Fb2Ns))?.Value.Trim();
//            var dateWritten = ti?.Element(XName.Get("date", Fb2Ns));
//            var yearWritten = dateWritten?.Attribute("value")?.Value.Trim() ?? dateWritten?.Value.Trim();
//            var uuid = di?.Element(XName.Get("id", Fb2Ns))?.Value.Trim();
//            var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");

//            return new BookMeta
//            {
//                Title = ti?.Element(XName.Get("book-title", Fb2Ns))?.Value.Trim(),
//                Lang = ti?.Element(XName.Get("lang", Fb2Ns))?.Value.Trim(),
//                Annotation = annotation,
//                Sequences = sequences?.Count > 0 ? sequences : null,
//                Authors = authors?.Count > 0 ? authors : null,
//                Written = (yearWritten != null || yearPublic != null)
//                    ? new WrittenInfo { Date = yearWritten, DatePublic = yearPublic }
//                    : null,
//                Uuid = uuid,
//                Created = now,
//                Updated = now,
//                Version = "1.1"
//            };
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // Сноски: body[@name='notes'] → словарь {id → ParsedNote}
//        // ══════════════════════════════════════════════════════════════════════

//        /// <summary>
//        /// Разбирает body[@name='notes'] в словарь {noteId → ParsedNote}.
//        /// xp сноски = [notesBodyIdx, sectionIdx, paragraphIdx], все 1-based.
//        /// notesBodyIdx — порядковый номер notes-body среди всех body документа.
//        /// </summary>
//        private static Dictionary<string, ParsedNote> ExtractNotes(XDocument doc)
//        {
//            var result = new Dictionary<string, ParsedNote>(StringComparer.Ordinal);

//            var allBodies = doc.Root?.Elements(NsBody).ToList() ?? [];
//            var notesBodyIndex = allBodies.FindIndex(b => b.Attribute("name")?.Value == "notes");
//            if (notesBodyIndex < 0) return result;

//            var notesBody = allBodies[notesBodyIndex];
//            int notesBodyIdx = notesBodyIndex + 1; // 1-based

//            int sectionIdx = 0;

//            foreach (var section in notesBody.Elements(NsSection))
//            {
//                sectionIdx++;
//                int mySectionIdx = sectionIdx;
//                int elemIdx = 0;

//                foreach (var child in section.Elements())
//                {
//                    if (child.Name == NsTitle) continue; // заголовок секции сносок — пропускаем

//                    if (child.Name != NsP) continue;

//                    elemIdx++;
//                    var id = child.Attribute("id")?.Value;

//                    if (string.IsNullOrEmpty(id))
//                    {
//                        // Параграф без id — продолжение предыдущей сноски
//                        var last = result.Values.LastOrDefault(n => n.Xp[1] == mySectionIdx);
//                        var para = ParseFootnoteParagraph(child);
//                        if (para != null) last?.Paragraphs.Add(para);
//                        continue;
//                    }

//                    var firstPara = ParseFootnoteParagraph(child);
//                    result[id] = new ParsedNote
//                    {
//                        NoteId = id,
//                        Xp = [notesBodyIdx, mySectionIdx, elemIdx],
//                        Paragraphs = firstPara != null ? [firstPara] : []
//                    };
//                }
//            }

//            return result;
//        }

//        /// <summary>
//        /// Возвращает plain text одного параграфа сноски.
//        /// </summary>
//        private static string? ParseFootnoteParagraph(XElement p)
//        {
//            var text = FlattenText(p);
//            return string.IsNullOrEmpty(text) ? null : text;
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // Основной body
//        // ══════════════════════════════════════════════════════════════════════

//        private static List<ParsedElement> ParseMainBody(
//            XDocument doc,
//            Dictionary<string, ParsedNote> notesLookup)
//        {
//            var elements = new List<ParsedElement>();

//            var mainBody = doc.Root?
//                .Elements(NsBody)
//                .FirstOrDefault(b => b.Attribute("name") == null
//                                  || b.Attribute("name")!.Value != "notes");

//            if (mainBody == null) return elements;

//            int globalIdx = 0;
//            int bodyIdx = 1;
//            int sectionIdx = 0;

//            foreach (var topSection in mainBody.Elements(NsSection))
//            {
//                sectionIdx++;
//                WalkSection(topSection, bodyIdx, ref sectionIdx,
//                            notesLookup, elements, ref globalIdx, isFirst: true);
//            }

//            // Прямые дочерние элементы body (без секции)
//            int directElem = 0;
//            foreach (var child in mainBody.Elements())
//            {
//                if (child.Name == NsSection) continue;
//                var pe = ParseElement(child, bodyIdx, 0, ++directElem,
//                                      notesLookup, ref globalIdx);
//                if (pe != null) elements.Add(pe);
//            }

//            return elements;
//        }

//        private static void WalkSection(
//            XElement section,
//            int bodyIdx,
//            ref int sectionIdx,
//            Dictionary<string, ParsedNote> notesLookup,
//            List<ParsedElement> result,
//            ref int globalIdx,
//            bool isFirst = false)
//        {
//            int mySectionIdx = sectionIdx;
//            int elemIdx = 0;

//            foreach (var child in section.Elements())
//            {
//                if (child.Name == NsSection)
//                {
//                    sectionIdx++;
//                    WalkSection(child, bodyIdx, ref sectionIdx,
//                                notesLookup, result, ref globalIdx);
//                    continue;
//                }

//                var pe = ParseElement(child, bodyIdx, mySectionIdx, ++elemIdx,
//                                      notesLookup, ref globalIdx);
//                if (pe != null) result.Add(pe);
//            }
//        }

//        private static ParsedElement? ParseElement(
//            XElement el,
//            int bodyIdx,
//            int sectionIdx,
//            int elemIdx,
//            Dictionary<string, ParsedNote> notesLookup,
//            ref int globalIdx)
//        {
//            string? type = null;
//            List<object>? mixed = null;   // смешанный контент (строки + NoteSegment)
//            string? flatText = null;

//            if (el.Name == NsP)
//            {
//                type = "p";
//                mixed = ExtractMixedContent(el, notesLookup);
//                flatText = MixedToPlainText(mixed);
//            }
//            else if (el.Name == NsTitle)
//            {
//                type = "title";
//                mixed = el.Elements(NsP)
//                    .SelectMany(p => ExtractMixedContent(p, notesLookup))
//                    .ToList();
//                if (mixed.Count == 0) mixed = null;
//                flatText = MixedToPlainText(mixed);
//            }
//            else if (el.Name == NsSubtitle)
//            {
//                type = "subtitle";
//                mixed = ExtractMixedContent(el, notesLookup);
//                flatText = MixedToPlainText(mixed);
//            }
//            else if (el.Name == NsEmptyLine)
//            {
//                type = "br";
//                mixed = null;
//                flatText = null;
//            }
//            else
//            {
//                return null; // img, binary и пр. — пропускаем
//            }

//            // Если в mixed нет ни одного NoteSegment — весь контент — просто строка
//            object? content = mixed == null ? null
//                : mixed.All(x => x is string) ? (object?)string.Concat(mixed.Cast<string>()).Trim().NullIfEmpty()
//                : mixed;

//            var elem = new ParsedElement
//            {
//                Type = type,
//                Content = content,
//                Text = flatText,
//                BodyIndex = bodyIdx,
//                SectionIndex = sectionIdx,
//                ElemIndex = elemIdx,
//                GlobalIndex = globalIdx
//            };

//            globalIdx++;
//            return elem;
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // Смешанный контент абзаца → List<TextSegment>
//        // ══════════════════════════════════════════════════════════════════════

//        /// <summary>
//        /// Разбирает смешанное содержимое элемента (текст + теги) в плоский список
//        /// TextSegment: чередование {t:"text", c:"..."} и {t:"note", ...}.
//        /// Соседние текстовые узлы объединяются в один сегмент.
//        /// </summary>
//        /// <summary>
//        /// Разбирает смешанное содержимое элемента в список объектов:
//        /// строки (plain text) чередуются с NoteSegment.
//        /// Возвращает List&lt;object&gt; где каждый элемент — string или NoteSegment.
//        /// </summary>
//        private static List<object> ExtractMixedContent(
//            XElement el,
//            Dictionary<string, ParsedNote> notesLookup)
//        {
//            var result = new List<object>();
//            var textBuf = new StringBuilder();

//            void FlushText()
//            {
//                if (textBuf.Length == 0) return;
//                var s = CollapseWhitespace(textBuf.ToString());
//                if (s.Length > 0)
//                    result.Add(s);
//                textBuf.Clear();
//            }

//            void WalkNodes(XElement parent)
//            {
//                foreach (var node in parent.Nodes())
//                {
//                    switch (node)
//                    {
//                        case XText txt:
//                            textBuf.Append(txt.Value);
//                            break;

//                        case XElement child when child.Name == NsA:
//                            {
//                                var noteType = child.Attribute("type")?.Value;
//                                var href = child.Attribute(XName.Get("href", XlinkNs))?.Value;

//                                if (noteType == "note" && href != null)
//                                {
//                                    var noteId = href.TrimStart('#');
//                                    // Видимая метка — то что написано внутри <a>: "[1]", "[15]" и т.д.
//                                    var label = child.Value;

//                                    FlushText(); // сначала сбрасываем накопленный текст

//                                    if (notesLookup.TryGetValue(noteId, out var note))
//                                    {
//                                        result.Add(new NoteSegment
//                                        {
//                                            C = label,
//                                            Xp = note.Xp,
//                                            F = new FootnoteContent
//                                            {
//                                                Xp = note.Xp,
//                                                C = note.Paragraphs
//                                            }
//                                        });
//                                    }
//                                    else
//                                    {
//                                        // Сноска не найдена — вставляем метку как обычный текст
//                                        textBuf.Append(label);
//                                    }
//                                }
//                                else
//                                {
//                                    // Обычная ссылка — берём текст
//                                    textBuf.Append(child.Value);
//                                }
//                                break;
//                            }

//                        case XElement child when child.Name == NsStrong
//                                              || child.Name == NsEmphasis:
//                            // Форматирование — рекурсивно разбираем содержимое
//                            // (форматирование пока теряется, текст сохраняется)
//                            WalkNodes(child);
//                            break;

//                        case XElement child:
//                            WalkNodes(child);
//                            break;
//                    }
//                }
//            }

//            WalkNodes(el);
//            FlushText();

//            return result;
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // TOC
//        // ══════════════════════════════════════════════════════════════════════

//        private static (TocDocument toc, List<TocChapterEntry> chapters) BuildToc(
//            BookMeta meta,
//            List<ParsedElement> elements,
//            ConversionOptions options)
//        {
//            if (elements.Count == 0)
//            {
//                return (new TocDocument
//                {
//                    Meta = meta,
//                    FullLength = 0,
//                    Body = [],
//                    Parts = []
//                }, []);
//            }

//            var chapters = BuildChapters(elements);
//            int firstS = elements[0].GlobalIndex;
//            int lastE = elements[^1].GlobalIndex;
//            var bookTitle = elements.FirstOrDefault(e => e.Type == "title")?.Text
//                ?? meta.Title ?? string.Empty;

//            var bodyEntry = new TocBodyEntry
//            {
//                S = firstS,
//                E = lastE,
//                T = bookTitle,
//                C = chapters
//            };

//            var parts = BuildParts(elements, options.TargetPartSize);
//            var tocDoc = new TocDocument
//            {
//                Meta = meta,
//                FullLength = CalculateFullLength(elements),
//                Body = [bodyEntry],
//                Parts = parts
//            };

//            return (tocDoc, chapters);
//        }

//        private static List<TocChapterEntry> BuildChapters(List<ParsedElement> elements)
//        {
//            var result = new List<TocChapterEntry>();
//            int? chapterStart = null;
//            string? chapterTitle = null;

//            var firstTitleIdx = elements.FindIndex(e => e.Type == "title");

//            if (firstTitleIdx > 0)
//            {
//                result.Add(new TocChapterEntry
//                {
//                    S = elements[0].GlobalIndex,
//                    E = elements[firstTitleIdx - 1].GlobalIndex
//                });
//            }

//            for (int i = firstTitleIdx < 0 ? 0 : firstTitleIdx; i < elements.Count; i++)
//            {
//                var el = elements[i];

//                if (el.Type == "title")
//                {
//                    if (chapterStart.HasValue && i > 0)
//                    {
//                        result.Add(new TocChapterEntry
//                        {
//                            S = chapterStart.Value,
//                            E = elements[i - 1].GlobalIndex,
//                            T = chapterTitle
//                        });
//                    }
//                    chapterStart = el.GlobalIndex;
//                    chapterTitle = el.Text;
//                }
//            }

//            if (chapterStart.HasValue)
//            {
//                result.Add(new TocChapterEntry
//                {
//                    S = chapterStart.Value,
//                    E = elements[^1].GlobalIndex,
//                    T = chapterTitle
//                });
//            }

//            return result;
//        }

//        private static List<TocPartEntry> BuildParts(List<ParsedElement> elements, int targetSize)
//        {
//            var parts = new List<TocPartEntry>();
//            int partIndex = 0;
//            int i = 0;

//            while (i < elements.Count)
//            {
//                int end = Math.Min(i + targetSize - 1, elements.Count - 1);
//                var first = elements[i];
//                var last = elements[end];

//                parts.Add(new TocPartEntry
//                {
//                    S = first.GlobalIndex,
//                    E = last.GlobalIndex,
//                    Xps = [first.BodyIndex, first.SectionIndex, first.ElemIndex],
//                    Xpe = [last.BodyIndex, last.SectionIndex, last.ElemIndex],
//                    Url = $"{partIndex:D3}.js"
//                });

//                partIndex++;
//                i = end + 1;
//            }

//            return parts;
//        }

//        // ══════════════════════════════════════════════════════════════════════
//        // Helpers
//        // ══════════════════════════════════════════════════════════════════════

//        private static int CalculateFullLength(List<ParsedElement> elements)
//            => elements.Sum(e => e.Text?.Length ?? 0);

//        private static string FlattenText(XElement el)
//        {
//            var raw = string.Concat(el.DescendantNodes()
//                .OfType<XText>().Select(t => t.Value));
//            return CollapseWhitespace(raw);
//        }

//        private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

//        private static string CollapseWhitespace(string s)
//            => WhitespaceRegex.Replace(s, " ").Trim();

//        /// <summary>Собирает плоский текст из смешанного списка (для TOC / FullLength).</summary>
//        private static string? MixedToPlainText(List<object>? mixed)
//        {
//            if (mixed == null || mixed.Count == 0) return null;
//            return string.Concat(mixed.OfType<string>()).Trim().NullIfEmpty();
//        }

//        private static PartElement MapToPartElement(ParsedElement pe)
//            => new()
//            {
//                T = pe.Type,
//                Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
//                C = pe.Content
//            };
//    }

//    // ── Вспомогательный internal-класс для хранения разобранной сноски ────────











//    //public sealed class Fb2ConverterService : IFb2Converter
//    //{
//    //    private const string Fb2Ns = "http://www.gribuser.ru/xml/fictionbook/2.0";
//    //    private const string XlinkNs = "http://www.w3.org/1999/xlink";

//    //    private static readonly XName NsP = XName.Get("p", Fb2Ns);
//    //    private static readonly XName NsA = XName.Get("a", Fb2Ns);
//    //    private static readonly XName NsStrong = XName.Get("strong", Fb2Ns);
//    //    private static readonly XName NsEmphasis = XName.Get("emphasis", Fb2Ns);
//    //    private static readonly XName NsSection = XName.Get("section", Fb2Ns);
//    //    private static readonly XName NsTitle = XName.Get("title", Fb2Ns);
//    //    private static readonly XName NsSubtitle = XName.Get("subtitle", Fb2Ns);
//    //    private static readonly XName NsBody = XName.Get("body", Fb2Ns);
//    //    private static readonly XName NsDescription = XName.Get("description", Fb2Ns);
//    //    private static readonly XName NsAnnotation = XName.Get("annotation", Fb2Ns);
//    //    private static readonly XName NsEmptyLine = XName.Get("empty-line", Fb2Ns);

//    //    private static readonly JsonSerializerOptions JsonOpts = new()
//    //    {
//    //        WriteIndented = true,
//    //        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//    //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
//    //    };

//    //    private readonly IBookStorage _storage;

//    //    public Fb2ConverterService(IBookStorage storage)
//    //    {
//    //        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // Public API
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    public async Task<ConversionResult> ConvertAsync(
//    //        Stream fb2Stream,
//    //        string? bookId = null,
//    //        ConversionOptions? options = null,
//    //        CancellationToken cancellationToken = default)
//    //    {
//    //        options ??= new ConversionOptions();

//    //        var doc = await LoadXmlAsync(fb2Stream, cancellationToken);

//    //        var rawMeta = ExtractMeta(doc);
//    //        var resolvedBookId = bookId ?? rawMeta.Uuid ?? Guid.NewGuid().ToString("D");

//    //        var meta = rawMeta with
//    //        {
//    //            Uuid = resolvedBookId,
//    //            Version = options.FormatVersion
//    //        };

//    //        // Словарь сносок: noteId → ParsedNote (с xp и параграфами)
//    //        var notes = ExtractNotes(doc);

//    //        // Основной обход → список элементов
//    //        var elements = ParseMainBody(doc, notes);

//    //        // TOC
//    //        var (tocDoc, _) = BuildToc(meta, elements, options);

//    //        // toc.json
//    //        var tocJson = JsonSerializer.Serialize(tocDoc, JsonOpts);
//    //        var tocBytes = Encoding.UTF8.GetByteCount(tocJson);
//    //        await _storage.SaveAsync(resolvedBookId, "toc.json", tocJson, cancellationToken);

//    //        var tocFileInfo = new StoredFileInfo
//    //        {
//    //            BookId = resolvedBookId,
//    //            FileName = "toc.json",
//    //            FileType = StoredFileType.Toc,
//    //            SizeBytes = tocBytes
//    //        };

//    //        // Фрагменты
//    //        var partFileInfos = new List<StoredFileInfo>();

//    //        foreach (var part in tocDoc.Parts)
//    //        {
//    //            cancellationToken.ThrowIfCancellationRequested();

//    //            var partElements = elements
//    //                .Where(e => e.GlobalIndex >= part.S && e.GlobalIndex <= part.E)
//    //                .Select(MapToPartElement)
//    //                .ToList();

//    //            var partJson = JsonSerializer.Serialize(partElements, JsonOpts);
//    //            var partBytes = Encoding.UTF8.GetByteCount(partJson);
//    //            await _storage.SaveAsync(resolvedBookId, part.Url, partJson, cancellationToken);

//    //            partFileInfos.Add(new StoredFileInfo
//    //            {
//    //                BookId = resolvedBookId,
//    //                FileName = part.Url,
//    //                FileType = StoredFileType.Part,
//    //                GlobalStart = part.S,
//    //                GlobalEnd = part.E,
//    //                XpStart = part.Xps,
//    //                XpEnd = part.Xpe,
//    //                SizeBytes = partBytes
//    //            });
//    //        }

//    //        return new ConversionResult
//    //        {
//    //            BookId = resolvedBookId,
//    //            Meta = meta,
//    //            TotalElements = elements.Count,
//    //            TocFile = tocFileInfo,
//    //            PartFiles = partFileInfos,
//    //            CompletedAt = DateTimeOffset.UtcNow
//    //        };
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // Загрузка XML
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    private static async Task<XDocument> LoadXmlAsync(Stream stream, CancellationToken ct)
//    //    {
//    //        using var ms = new MemoryStream();
//    //        await stream.CopyToAsync(ms, ct);
//    //        ms.Position = 0;

//    //        var settings = new XmlReaderSettings
//    //        {
//    //            DtdProcessing = DtdProcessing.Ignore,
//    //            IgnoreWhitespace = false,
//    //            Async = true
//    //        };
//    //        using var reader = XmlReader.Create(ms, settings);
//    //        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // Метаданные
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    private static BookMeta ExtractMeta(XDocument doc)
//    //    {
//    //        var desc = doc.Root?.Element(NsDescription);
//    //        var ti = desc?.Element(XName.Get("title-info", Fb2Ns));
//    //        var di = desc?.Element(XName.Get("document-info", Fb2Ns));

//    //        var authors = ti?.Elements(XName.Get("author", Fb2Ns))
//    //            .Select(a => new AuthorInfo
//    //            {
//    //                Role = "author",
//    //                First = a.Element(XName.Get("first-name", Fb2Ns))?.Value.Trim(),
//    //                Last = a.Element(XName.Get("last-name", Fb2Ns))?.Value.Trim()
//    //            }).ToList();

//    //        var sequences = ti?.Elements(XName.Get("sequence", Fb2Ns))
//    //            .Select(s => s.Attribute("name")?.Value)
//    //            .Where(n => n != null).Cast<string>().ToList();

//    //        var annotation = ti?.Element(NsAnnotation) is XElement ann
//    //            ? string.Join(" ", ann.Descendants(NsP).Select(FlattenText)).Trim()
//    //            : null;

//    //        var publishInfo = desc?.Element(XName.Get("publish-info", Fb2Ns));
//    //        var yearPublic = publishInfo?.Element(XName.Get("year", Fb2Ns))?.Value.Trim();
//    //        var dateWritten = ti?.Element(XName.Get("date", Fb2Ns));
//    //        var yearWritten = dateWritten?.Attribute("value")?.Value.Trim() ?? dateWritten?.Value.Trim();
//    //        var uuid = di?.Element(XName.Get("id", Fb2Ns))?.Value.Trim();
//    //        var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");

//    //        return new BookMeta
//    //        {
//    //            Title = ti?.Element(XName.Get("book-title", Fb2Ns))?.Value.Trim(),
//    //            Lang = ti?.Element(XName.Get("lang", Fb2Ns))?.Value.Trim(),
//    //            Annotation = annotation,
//    //            Sequences = sequences?.Count > 0 ? sequences : null,
//    //            Authors = authors?.Count > 0 ? authors : null,
//    //            Written = (yearWritten != null || yearPublic != null)
//    //                ? new WrittenInfo { Date = yearWritten, DatePublic = yearPublic }
//    //                : null,
//    //            Uuid = uuid,
//    //            Created = now,
//    //            Updated = now,
//    //            Version = "1.1"
//    //        };
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // Сноски: body[@name='notes'] → словарь {id → ParsedNote}
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    /// <summary>
//    //    /// Разбирает body[@name='notes'] в словарь {noteId → ParsedNote}.
//    //    /// xp сноски = [notesBodyIdx, sectionIdx, paragraphIdx], все 1-based.
//    //    /// notesBodyIdx — порядковый номер notes-body среди всех body документа.
//    //    /// </summary>
//    //    private static Dictionary<string, ParsedNote> ExtractNotes(XDocument doc)
//    //    {
//    //        var result = new Dictionary<string, ParsedNote>(StringComparer.Ordinal);

//    //        var allBodies = doc.Root?.Elements(NsBody).ToList() ?? [];
//    //        var notesBodyIndex = allBodies.FindIndex(b => b.Attribute("name")?.Value == "notes");
//    //        if (notesBodyIndex < 0) return result;

//    //        var notesBody = allBodies[notesBodyIndex];
//    //        int notesBodyIdx = notesBodyIndex + 1; // 1-based

//    //        int sectionIdx = 0;

//    //        foreach (var section in notesBody.Elements(NsSection))
//    //        {
//    //            sectionIdx++;
//    //            int mySectionIdx = sectionIdx;
//    //            int elemIdx = 0;

//    //            foreach (var child in section.Elements())
//    //            {
//    //                if (child.Name == NsTitle) continue; // заголовок секции сносок — пропускаем

//    //                if (child.Name != NsP) continue;

//    //                elemIdx++;
//    //                var id = child.Attribute("id")?.Value;

//    //                if (string.IsNullOrEmpty(id))
//    //                {
//    //                    // Параграф без id — продолжение предыдущей сноски
//    //                    var last = result.Values.LastOrDefault(n => n.Xp[1] == mySectionIdx);
//    //                    last?.Paragraphs.Add(ParseFootnoteParagraph(child));
//    //                    continue;
//    //                }

//    //                result[id] = new ParsedNote
//    //                {
//    //                    NoteId = id,
//    //                    Xp = [notesBodyIdx, mySectionIdx, elemIdx],
//    //                    Paragraphs = [ParseFootnoteParagraph(child)]
//    //                };
//    //            }
//    //        }

//    //        return result;
//    //    }

//    //    /// <summary>
//    //    /// Разбирает один параграф тела сноски в List&lt;TextSegment&gt;.
//    //    /// </summary>
//    //    private static List<TextSegment> ParseFootnoteParagraph(XElement p)
//    //    {
//    //        var text = FlattenText(p);
//    //        if (string.IsNullOrEmpty(text)) return [];
//    //        return [new TextSegment { T = "text", C = text }];
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // Основной body
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    private static List<ParsedElement> ParseMainBody(
//    //        XDocument doc,
//    //        Dictionary<string, ParsedNote> notesLookup)
//    //    {
//    //        var elements = new List<ParsedElement>();

//    //        var mainBody = doc.Root?
//    //            .Elements(NsBody)
//    //            .FirstOrDefault(b => b.Attribute("name") == null
//    //                              || b.Attribute("name")!.Value != "notes");

//    //        if (mainBody == null) return elements;

//    //        int globalIdx = 0;
//    //        int bodyIdx = 1;
//    //        int sectionIdx = 0;

//    //        foreach (var topSection in mainBody.Elements(NsSection))
//    //        {
//    //            sectionIdx++;
//    //            WalkSection(topSection, bodyIdx, ref sectionIdx,
//    //                        notesLookup, elements, ref globalIdx, isFirst: true);
//    //        }

//    //        // Прямые дочерние элементы body (без секции)
//    //        int directElem = 0;
//    //        foreach (var child in mainBody.Elements())
//    //        {
//    //            if (child.Name == NsSection) continue;
//    //            var pe = ParseElement(child, bodyIdx, 0, ++directElem,
//    //                                  notesLookup, ref globalIdx);
//    //            if (pe != null) elements.Add(pe);
//    //        }

//    //        return elements;
//    //    }

//    //    private static void WalkSection(
//    //        XElement section,
//    //        int bodyIdx,
//    //        ref int sectionIdx,
//    //        Dictionary<string, ParsedNote> notesLookup,
//    //        List<ParsedElement> result,
//    //        ref int globalIdx,
//    //        bool isFirst = false)
//    //    {
//    //        int mySectionIdx = sectionIdx;
//    //        int elemIdx = 0;

//    //        foreach (var child in section.Elements())
//    //        {
//    //            if (child.Name == NsSection)
//    //            {
//    //                sectionIdx++;
//    //                WalkSection(child, bodyIdx, ref sectionIdx,
//    //                            notesLookup, result, ref globalIdx);
//    //                continue;
//    //            }

//    //            var pe = ParseElement(child, bodyIdx, mySectionIdx, ++elemIdx,
//    //                                  notesLookup, ref globalIdx);
//    //            if (pe != null) result.Add(pe);
//    //        }
//    //    }

//    //    private static ParsedElement? ParseElement(
//    //        XElement el,
//    //        int bodyIdx,
//    //        int sectionIdx,
//    //        int elemIdx,
//    //        Dictionary<string, ParsedNote> notesLookup,
//    //        ref int globalIdx)
//    //    {
//    //        string? type = null;
//    //        List<TextSegment>? segments = null;
//    //        string? flatText = null;

//    //        if (el.Name == NsP)
//    //        {
//    //            type = "p";
//    //            segments = ExtractMixedContent(el, notesLookup);
//    //            flatText = SegmentsToPlainText(segments);
//    //        }
//    //        else if (el.Name == NsTitle)
//    //        {
//    //            type = "title";
//    //            // <title> содержит один или несколько <p>
//    //            var allSegs = el.Elements(NsP)
//    //                .SelectMany(p => ExtractMixedContent(p, notesLookup))
//    //                .ToList();
//    //            segments = allSegs.Count > 0 ? allSegs : null;
//    //            flatText = SegmentsToPlainText(segments);
//    //        }
//    //        else if (el.Name == NsSubtitle)
//    //        {
//    //            type = "subtitle";
//    //            segments = ExtractMixedContent(el, notesLookup);
//    //            flatText = SegmentsToPlainText(segments);
//    //        }
//    //        else if (el.Name == NsEmptyLine)
//    //        {
//    //            type = "br";
//    //            segments = null;
//    //            flatText = null;
//    //        }
//    //        else
//    //        {
//    //            return null; // img, binary и пр. — пропускаем
//    //        }

//    //        var elem = new ParsedElement
//    //        {
//    //            Type = type,
//    //            Segments = segments,
//    //            Text = flatText,
//    //            BodyIndex = bodyIdx,
//    //            SectionIndex = sectionIdx,
//    //            ElemIndex = elemIdx,
//    //            GlobalIndex = globalIdx
//    //        };

//    //        globalIdx++;
//    //        return elem;
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // Смешанный контент абзаца → List<TextSegment>
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    /// <summary>
//    //    /// Разбирает смешанное содержимое элемента (текст + теги) в плоский список
//    //    /// TextSegment: чередование {t:"text", c:"..."} и {t:"note", ...}.
//    //    /// Соседние текстовые узлы объединяются в один сегмент.
//    //    /// </summary>
//    //    private static List<TextSegment> ExtractMixedContent(
//    //        XElement el,
//    //        Dictionary<string, ParsedNote> notesLookup)
//    //    {
//    //        var result = new List<TextSegment>();
//    //        var textBuf = new StringBuilder();

//    //        void FlushText()
//    //        {
//    //            if (textBuf.Length == 0) return;
//    //            var s = CollapseWhitespace(textBuf.ToString());
//    //            if (s.Length > 0)
//    //                result.Add(new TextSegment { T = "text", C = s });
//    //            textBuf.Clear();
//    //        }

//    //        void WalkNodes(XElement parent)
//    //        {
//    //            foreach (var node in parent.Nodes())
//    //            {
//    //                switch (node)
//    //                {
//    //                    case XText txt:
//    //                        textBuf.Append(txt.Value);
//    //                        break;

//    //                    case XElement child when child.Name == NsA:
//    //                        {
//    //                            var noteType = child.Attribute("type")?.Value;
//    //                            var href = child.Attribute(XName.Get("href", XlinkNs))?.Value;

//    //                            if (noteType == "note" && href != null)
//    //                            {
//    //                                var noteId = href.TrimStart('#');
//    //                                // Видимая метка — то что написано внутри <a>: "[1]", "[15]" и т.д.
//    //                                var label = child.Value;

//    //                                FlushText(); // сначала сбрасываем накопленный текст

//    //                                if (notesLookup.TryGetValue(noteId, out var note))
//    //                                {
//    //                                    result.Add(new TextSegment
//    //                                    {
//    //                                        T = "note",
//    //                                        Role = "footnote",
//    //                                        C = label,
//    //                                        Xp = note.Xp,
//    //                                        F = new FootnoteContent
//    //                                        {
//    //                                            Xp = note.Xp,
//    //                                            C = note.Paragraphs
//    //                                        }
//    //                                    });
//    //                                }
//    //                                else
//    //                                {
//    //                                    // Сноска не найдена — вставляем метку как обычный текст
//    //                                    textBuf.Append(label);
//    //                                }
//    //                            }
//    //                            else
//    //                            {
//    //                                // Обычная ссылка — берём текст
//    //                                textBuf.Append(child.Value);
//    //                            }
//    //                            break;
//    //                        }

//    //                    case XElement child when child.Name == NsStrong
//    //                                          || child.Name == NsEmphasis:
//    //                        // Форматирование — рекурсивно разбираем содержимое
//    //                        // (форматирование пока теряется, текст сохраняется)
//    //                        WalkNodes(child);
//    //                        break;

//    //                    case XElement child:
//    //                        WalkNodes(child);
//    //                        break;
//    //                }
//    //            }
//    //        }

//    //        WalkNodes(el);
//    //        FlushText();

//    //        return result;
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // TOC
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    private static (TocDocument toc, List<TocChapterEntry> chapters) BuildToc(
//    //        BookMeta meta,
//    //        List<ParsedElement> elements,
//    //        ConversionOptions options)
//    //    {
//    //        if (elements.Count == 0)
//    //        {
//    //            return (new TocDocument
//    //            {
//    //                Meta = meta,
//    //                FullLength = 0,
//    //                Body = [],
//    //                Parts = []
//    //            }, []);
//    //        }

//    //        var chapters = BuildChapters(elements);
//    //        int firstS = elements[0].GlobalIndex;
//    //        int lastE = elements[^1].GlobalIndex;
//    //        var bookTitle = elements.FirstOrDefault(e => e.Type == "title")?.Text
//    //            ?? meta.Title ?? string.Empty;

//    //        var bodyEntry = new TocBodyEntry
//    //        {
//    //            S = firstS,
//    //            E = lastE,
//    //            T = bookTitle,
//    //            C = chapters
//    //        };

//    //        var parts = BuildParts(elements, options.TargetPartSize);
//    //        var tocDoc = new TocDocument
//    //        {
//    //            Meta = meta,
//    //            FullLength = CalculateFullLength(elements),
//    //            Body = [bodyEntry],
//    //            Parts = parts
//    //        };

//    //        return (tocDoc, chapters);
//    //    }

//    //    private static List<TocChapterEntry> BuildChapters(List<ParsedElement> elements)
//    //    {
//    //        var result = new List<TocChapterEntry>();
//    //        int? chapterStart = null;
//    //        string? chapterTitle = null;

//    //        var firstTitleIdx = elements.FindIndex(e => e.Type == "title");

//    //        if (firstTitleIdx > 0)
//    //        {
//    //            result.Add(new TocChapterEntry
//    //            {
//    //                S = elements[0].GlobalIndex,
//    //                E = elements[firstTitleIdx - 1].GlobalIndex
//    //            });
//    //        }

//    //        for (int i = firstTitleIdx < 0 ? 0 : firstTitleIdx; i < elements.Count; i++)
//    //        {
//    //            var el = elements[i];

//    //            if (el.Type == "title")
//    //            {
//    //                if (chapterStart.HasValue && i > 0)
//    //                {
//    //                    result.Add(new TocChapterEntry
//    //                    {
//    //                        S = chapterStart.Value,
//    //                        E = elements[i - 1].GlobalIndex,
//    //                        T = chapterTitle
//    //                    });
//    //                }
//    //                chapterStart = el.GlobalIndex;
//    //                chapterTitle = el.Text;
//    //            }
//    //        }

//    //        if (chapterStart.HasValue)
//    //        {
//    //            result.Add(new TocChapterEntry
//    //            {
//    //                S = chapterStart.Value,
//    //                E = elements[^1].GlobalIndex,
//    //                T = chapterTitle
//    //            });
//    //        }

//    //        return result;
//    //    }

//    //    private static List<TocPartEntry> BuildParts(List<ParsedElement> elements, int targetSize)
//    //    {
//    //        var parts = new List<TocPartEntry>();
//    //        int partIndex = 0;
//    //        int i = 0;

//    //        while (i < elements.Count)
//    //        {
//    //            int end = Math.Min(i + targetSize - 1, elements.Count - 1);
//    //            var first = elements[i];
//    //            var last = elements[end];

//    //            parts.Add(new TocPartEntry
//    //            {
//    //                S = first.GlobalIndex,
//    //                E = last.GlobalIndex,
//    //                Xps = [first.BodyIndex, first.SectionIndex, first.ElemIndex],
//    //                Xpe = [last.BodyIndex, last.SectionIndex, last.ElemIndex],
//    //                Url = $"{partIndex:D3}.js"
//    //            });

//    //            partIndex++;
//    //            i = end + 1;
//    //        }

//    //        return parts;
//    //    }

//    //    // ══════════════════════════════════════════════════════════════════════
//    //    // Helpers
//    //    // ══════════════════════════════════════════════════════════════════════

//    //    private static int CalculateFullLength(List<ParsedElement> elements)
//    //        => elements.Sum(e => e.Text?.Length ?? 0);

//    //    private static string FlattenText(XElement el)
//    //    {
//    //        var raw = string.Concat(el.DescendantNodes()
//    //            .OfType<XText>().Select(t => t.Value));
//    //        return CollapseWhitespace(raw);
//    //    }

//    //    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

//    //    private static string CollapseWhitespace(string s)
//    //        => WhitespaceRegex.Replace(s, " ").Trim();

//    //    /// <summary>Собирает плоский текст из списка сегментов (для TOC / FullLength).</summary>
//    //    private static string? SegmentsToPlainText(List<TextSegment>? segments)
//    //    {
//    //        if (segments == null || segments.Count == 0) return null;
//    //        return string.Concat(segments
//    //            .Where(s => s.T == "text")
//    //            .Select(s => s.C ?? ""))
//    //            .Trim()
//    //            .NullIfEmpty();
//    //    }

//    //    private static PartElement MapToPartElement(ParsedElement pe)
//    //        => new()
//    //        {
//    //            T = pe.Type,
//    //            Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
//    //            C = pe.Segments
//    //        };
//    //}

//    //// ── Вспомогательный internal-класс для хранения разобранной сноски ────────

//    //internal sealed class ParsedNote
//    //{
//    //    public required string NoteId { get; init; }
//    //    /// <summary>[notesBodyIdx, sectionIdx, elemIdx] — 1-based.</summary>
//    //    public required int[] Xp { get; init; }
//    //    /// <summary>Параграфы тела сноски.</summary>
//    //    public required List<List<TextSegment>> Paragraphs { get; init; }
//    //}

//    //internal static class StringExtensions
//    //{
//    //    public static string? NullIfEmpty(this string s)
//    //        => string.IsNullOrEmpty(s) ? null : s;
//    //}






//    /// <summary>
//    /// Реализация конвертера FB2 в набор JSON-фрагментов
//    ///
//    /// Алгоритм:
//    /// 1. Разбирается XML
//    /// 2. Собирается словарь сносок из body[@name='notes']
//    /// 3. Обходится основной body, генерируется список <see cref="ParsedElement"/>
//    ///    со сквозной нумерацией и XP-координатами
//    /// 4. Формируется toc.json (Body + Parts по TargetPartSize элементов)
//    /// 5. Сериализуется каждый фрагмент в JSON и сохраняется через IBookStorage
//    /// 6. Возвращается ConversionResult с метаданными для записи в БД
//    /// </summary>
//    //public sealed class Fb2ConverterService : IFb2Converter
//    //{
//    //    private const string Fb2Ns = "http://www.gribuser.ru/xml/fictionbook/2.0";
//    //    private const string XlinkNs = "http://www.w3.org/1999/xlink";

//    //    private static readonly XName NsP = XName.Get("p", Fb2Ns);
//    //    private static readonly XName NsA = XName.Get("a", Fb2Ns);
//    //    private static readonly XName NsStrong = XName.Get("strong", Fb2Ns);
//    //    private static readonly XName NsEmphasis = XName.Get("emphasis", Fb2Ns);
//    //    private static readonly XName NsSection = XName.Get("section", Fb2Ns);
//    //    private static readonly XName NsTitle = XName.Get("title", Fb2Ns);
//    //    private static readonly XName NsSubtitle = XName.Get("subtitle", Fb2Ns);
//    //    private static readonly XName NsBody = XName.Get("body", Fb2Ns);
//    //    private static readonly XName NsDescription = XName.Get("description", Fb2Ns);
//    //    private static readonly XName NsAnnotation = XName.Get("annotation", Fb2Ns);
//    //    private static readonly XName NsEmptyLine = XName.Get("empty-line", Fb2Ns);

//    //    private static readonly JsonSerializerOptions JsonOpts = new()
//    //    {
//    //        WriteIndented = true,
//    //        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
//    //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
//    //    };

//    //    private readonly IBookStorage _storage;

//    //    public Fb2ConverterService(IBookStorage storage)
//    //    {
//    //        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
//    //    }


//    //    public async Task<ConversionResult> ConvertAsync(
//    //        Stream fb2Stream,
//    //        string? bookId = null,
//    //        ConversionOptions? options = null,
//    //        CancellationToken cancellationToken = default)
//    //    {
//    //        options ??= new ConversionOptions();

//    //        var doc = await LoadXmlAsync(fb2Stream, cancellationToken);

//    //        var rawMeta = ExtractMeta(doc);
//    //        var resolvedBookId = bookId
//    //            ?? rawMeta.Uuid
//    //            ?? Guid.NewGuid().ToString("D");

//    //        var meta = rawMeta with
//    //        {
//    //            Uuid = resolvedBookId,
//    //            Version = options.FormatVersion
//    //        };

//    //        // 3. Сноски: id → plaintext
//    //        var notes = ExtractNotes(doc);

//    //        // 4. Основной обход → список элементов
//    //        var elements = ParseMainBody(doc, notes);

//    //        // 5. Строим TOC
//    //        var (tocDoc, chapters) = BuildToc(meta, elements, options);

//    //        // 6. Сериализуем toc.json и сохраняем
//    //        var tocJson = JsonSerializer.Serialize(tocDoc, JsonOpts);
//    //        var tocBytes = Encoding.UTF8.GetByteCount(tocJson);
//    //        await _storage.SaveAsync(resolvedBookId, "toc.json", tocJson, cancellationToken);

//    //        var tocFileInfo = new StoredFileInfo
//    //        {
//    //            BookId = resolvedBookId,
//    //            FileName = "toc.json",
//    //            FileType = StoredFileType.Toc,
//    //            SizeBytes = tocBytes
//    //        };

//    //        // 7. Разбиваем на фрагменты и сохраняем каждый
//    //        var partFileInfos = new List<StoredFileInfo>();

//    //        foreach (var part in tocDoc.Parts)
//    //        {
//    //            cancellationToken.ThrowIfCancellationRequested();

//    //            var partElements = elements
//    //                .Where(e => e.GlobalIndex >= part.S && e.GlobalIndex <= part.E)
//    //                .Select(MapToPartElement)
//    //                .ToList();

//    //            var partJson = JsonSerializer.Serialize(partElements, JsonOpts);
//    //            var partBytes = Encoding.UTF8.GetByteCount(partJson);
//    //            await _storage.SaveAsync(resolvedBookId, part.Url, partJson, cancellationToken);

//    //            partFileInfos.Add(new StoredFileInfo
//    //            {
//    //                BookId = resolvedBookId,
//    //                FileName = part.Url,
//    //                FileType = StoredFileType.Part,
//    //                GlobalStart = part.S,
//    //                GlobalEnd = part.E,
//    //                XpStart = part.Xps,
//    //                XpEnd = part.Xpe,
//    //                SizeBytes = partBytes
//    //            });
//    //        }

//    //        return new ConversionResult
//    //        {
//    //            BookId = resolvedBookId,
//    //            Meta = meta,
//    //            TotalElements = elements.Count,
//    //            TocFile = tocFileInfo,
//    //            PartFiles = partFileInfos,
//    //            CompletedAt = DateTimeOffset.UtcNow
//    //        };
//    //    }

//    //    //Загрузка файла из Stream в XDocument

//    //    private static async Task<XDocument> LoadXmlAsync(
//    //        Stream stream, CancellationToken ct)
//    //    {

//    //        using var ms = new MemoryStream();
//    //        await stream.CopyToAsync(ms, ct);
//    //        ms.Position = 0;

//    //        var settings = new XmlReaderSettings
//    //        {
//    //            DtdProcessing = DtdProcessing.Ignore,
//    //            IgnoreWhitespace = false,
//    //            Async = true
//    //        };
//    //        using var reader = XmlReader.Create(ms, settings);
//    //        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
//    //    }

//    //    //Извлечение метаданных

//    //    private static BookMeta ExtractMeta(XDocument doc)
//    //    {
//    //        var desc = doc.Root?.Element(NsDescription);
//    //        var ti = desc?.Element(XName.Get("title-info", Fb2Ns));
//    //        var di = desc?.Element(XName.Get("document-info", Fb2Ns));

//    //        //Авторы
//    //        var authors = ti?.Elements(XName.Get("author", Fb2Ns))
//    //            .Select(a => new AuthorInfo
//    //            {
//    //                Role = "author",
//    //                First = a.Element(XName.Get("first-name", Fb2Ns))?.Value.Trim(),
//    //                Last = a.Element(XName.Get("last-name", Fb2Ns))?.Value.Trim()
//    //            })
//    //            .ToList();

//    //        //Серия
//    //        var sequences = ti?.Elements(XName.Get("sequence", Fb2Ns))
//    //            .Select(s => s.Attribute("name")?.Value)
//    //            .Where(n => n != null)
//    //            .Cast<string>()
//    //            .ToList();

//    //        //Аннотация — рекурсивный текст всех дочерних <p>
//    //        var annotation = ti?.Element(NsAnnotation) is XElement ann
//    //            ? string.Join(" ", ann.Descendants(NsP).Select(FlattenText)).Trim()
//    //            : null;

//    //        // Год написания / публикации из publish-info или title-info/date
//    //        var publishInfo = desc?.Element(XName.Get("publish-info", Fb2Ns));
//    //        var yearPublic = publishInfo?.Element(XName.Get("year", Fb2Ns))?.Value.Trim();
//    //        var dateWritten = ti?.Element(XName.Get("date", Fb2Ns));
//    //        var yearWritten = dateWritten?.Attribute("value")?.Value.Trim()
//    //            ?? dateWritten?.Value.Trim();

//    //        // UUID книги из document-info/id
//    //        var uuid = di?.Element(XName.Get("id", Fb2Ns))?.Value.Trim();

//    //        var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");

//    //        return new BookMeta
//    //        {
//    //            Title = ti?.Element(XName.Get("book-title", Fb2Ns))?.Value.Trim(),
//    //            Lang = ti?.Element(XName.Get("lang", Fb2Ns))?.Value.Trim(),
//    //            Annotation = annotation,
//    //            Sequences = sequences?.Count > 0 ? sequences : null,
//    //            Authors = authors?.Count > 0 ? authors : null,
//    //            Written = (yearWritten != null || yearPublic != null)
//    //                ? new WrittenInfo { Date = yearWritten, DatePublic = yearPublic }
//    //                : null,
//    //            Uuid = uuid,
//    //            Created = now,
//    //            Updated = now,
//    //            Version = "1.1"
//    //        };
//    //    }

//    //    /// <summary>
//    //    /// Извлекает все сноски из body[@name='notes'] в словарь {id → plaintext}
//    //    /// </summary>
//    //    private static Dictionary<string, string> ExtractNotes(XDocument doc)
//    //    {
//    //        var result = new Dictionary<string, string>(StringComparer.Ordinal);

//    //        var notesBody = doc.Root?
//    //            .Elements(NsBody)
//    //            .FirstOrDefault(b => b.Attribute("name")?.Value == "notes");

//    //        if (notesBody == null) return result;

//    //        // Все <p id="nX"> во всех секциях body notes
//    //        foreach (var p in notesBody.Descendants(NsP))
//    //        {
//    //            var id = p.Attribute("id")?.Value;
//    //            if (string.IsNullOrEmpty(id)) continue;
//    //            result[id] = FlattenText(p);
//    //        }

//    //        // Также title-секции самих секций (иногда сноска — целая <section id="nX">)
//    //        foreach (var sect in notesBody.Descendants(NsSection))
//    //        {
//    //            var id = sect.Attribute("id")?.Value;
//    //            if (string.IsNullOrEmpty(id)) continue;
//    //            if (!result.ContainsKey(id))
//    //                result[id] = FlattenText(sect);
//    //        }

//    //        return result;
//    //    }

//    //    /// <summary>
//    //    /// Обходит основной body (первый body без атрибута name="notes"),
//    //    /// возвращает плоский список <see cref="ParsedElement"/> со сквозной нумерацией
//    //    /// </summary>
//    //    private static List<ParsedElement> ParseMainBody(
//    //        XDocument doc,
//    //        Dictionary<string, string> notesLookup)
//    //    {
//    //        var elements = new List<ParsedElement>();

//    //        var mainBody = doc.Root?
//    //            .Elements(NsBody)
//    //            .FirstOrDefault(b => b.Attribute("name") == null
//    //                                 || b.Attribute("name")!.Value != "notes");

//    //        if (mainBody == null) return elements;

//    //        int globalIdx = 0;
//    //        int bodyIdx = 1;        // первый body = 1
//    //        int sectionIdx = 0;     // считаем все секции по дереву

//    //        // Рекурсивно обходим секции
//    //        foreach (var topSection in mainBody.Elements(NsSection))
//    //        {
//    //            sectionIdx++;
//    //            WalkSection(topSection, bodyIdx, ref sectionIdx,
//    //                        notesLookup, elements, ref globalIdx, isFirst: true);
//    //        }

//    //        // Если в body есть прямые <title> или <p> (без section-обёртки)
//    //        int directElem = 0;
//    //        foreach (var child in mainBody.Elements())
//    //        {
//    //            if (child.Name == NsSection) continue; // уже обработаны
//    //            var pe = ParseElement(child, bodyIdx, 0, ++directElem,
//    //                                  notesLookup, ref globalIdx);
//    //            if (pe != null) elements.Add(pe);
//    //        }

//    //        return elements;
//    //    }

//    //    /// <summary>
//    //    /// Рекурсивно обходит секцию:
//    //    ///   title-элемент секции в "title"
//    //    ///   subtitle           в "subtitle"
//    //    ///   p                  в "p"
//    //    ///   empty-line         в "br"
//    //    ///   вложенные секции   в рекурсия (sectionIdx инкрементируется)
//    //    /// </summary>
//    //    private static void WalkSection(
//    //        XElement section,
//    //        int bodyIdx,
//    //        ref int sectionIdx,
//    //        Dictionary<string, string> notesLookup,
//    //        List<ParsedElement> result,
//    //        ref int globalIdx,
//    //        bool isFirst = false)
//    //    {
//    //        int mySectionIdx = sectionIdx;
//    //        int elemIdx = 0;

//    //        foreach (var child in section.Elements())
//    //        {
//    //            if (child.Name == NsSection)
//    //            {
//    //                // Вложенная секция получает новый sectionIdx
//    //                sectionIdx++;
//    //                WalkSection(child, bodyIdx, ref sectionIdx,
//    //                            notesLookup, result, ref globalIdx);
//    //                continue;
//    //            }

//    //            var pe = ParseElement(child, bodyIdx, mySectionIdx, ++elemIdx,
//    //                                  notesLookup, ref globalIdx);
//    //            if (pe != null) result.Add(pe);
//    //        }
//    //    }

//    //    /// <summary>
//    //    /// Превращает один дочерний XML-элемент секции в <see cref="ParsedElement"/>
//    //    /// Возвращает null, если элемент не подлежит включению (img, binary и др.)
//    //    /// </summary>
//    //    private static ParsedElement? ParseElement(
//    //        XElement el,
//    //        int bodyIdx,
//    //        int sectionIdx,
//    //        int elemIdx,
//    //        Dictionary<string, string> notesLookup,
//    //        ref int globalIdx)
//    //    {
//    //        string? type = null;
//    //        string? text = null;
//    //        Dictionary<string, string>? inlineNotes = null;

//    //        if (el.Name == NsP)
//    //        {
//    //            type = "p";
//    //            (text, inlineNotes) = ExtractParagraphContent(el, notesLookup);
//    //        }
//    //        else if (el.Name == NsTitle)
//    //        {
//    //            type = "title";
//    //            // Внутри <title> обычно один или несколько <p>
//    //            text = string.Join(" ",
//    //                el.Descendants(NsP).Select(FlattenText).Where(s => s.Length > 0));
//    //        }
//    //        else if (el.Name == NsSubtitle)
//    //        {
//    //            type = "subtitle";
//    //            text = FlattenText(el);
//    //        }
//    //        else if (el.Name == NsEmptyLine)
//    //        {
//    //            type = "br";
//    //            text = null;
//    //        }
//    //        else
//    //        {
//    //            // Неизвестный / не текстовый тег — пропускаем без увеличения счётчика
//    //            return null;
//    //        }

//    //        var elem = new ParsedElement
//    //        {
//    //            Type = type,
//    //            Text = text?.Length > 0 ? text : null,
//    //            Notes = inlineNotes?.Count > 0 ? inlineNotes : null,
//    //            BodyIndex = bodyIdx,
//    //            SectionIndex = sectionIdx,
//    //            ElemIndex = elemIdx,
//    //            GlobalIndex = globalIdx
//    //        };

//    //        globalIdx++;
//    //        return elem;
//    //    }

//    //    /// <summary>
//    //    /// Извлекает текст абзаца и встроенные сноски.
//    //    /// Ссылки на сноску (<a type="note" href="#nX">) заменяются маркером [N],
//    //    /// а тексты сносок собираются в словарь.
//    //    /// </summary>
//    //    private static (string text, Dictionary<string, string>? notes)
//    //        ExtractParagraphContent(XElement p, Dictionary<string, string> notesLookup)
//    //    {
//    //        var sb = new StringBuilder();
//    //        Dictionary<string, string>? notes = null;
//    //        int noteCounter = 1;

//    //        ExtractMixed(p, sb, notesLookup, ref notes, ref noteCounter);

//    //        return (sb.ToString().Trim(), notes);
//    //    }

//    //    /// <summary>
//    //    /// Рекурсивно обходит смешанное содержимое (текст + дочерние теги),
//    //    /// накапливая плоский текст и собирая сноски
//    //    /// </summary>
//    //    private static void ExtractMixed(
//    //        XElement el,
//    //        StringBuilder sb,
//    //        Dictionary<string, string> notesLookup,
//    //        ref Dictionary<string, string>? notes,
//    //        ref int noteCounter)
//    //    {
//    //        foreach (var node in el.Nodes())
//    //        {
//    //            switch (node)
//    //            {
//    //                case XText textNode:
//    //                    sb.Append(textNode.Value);
//    //                    break;

//    //                case XElement child when child.Name == NsA:
//    //                    {
//    //                        // Inline note reference
//    //                        var noteType = child.Attribute("type")?.Value;
//    //                        var href = child.Attribute(XName.Get("href", XlinkNs))?.Value;

//    //                        if (noteType == "note" && href != null)
//    //                        {
//    //                            // href = "#n1" → id = "n1"
//    //                            var noteId = href.TrimStart('#');

//    //                            if (notesLookup.TryGetValue(noteId, out var noteText))
//    //                            {
//    //                                notes ??= new Dictionary<string, string>(StringComparer.Ordinal);
//    //                                var key = noteCounter.ToString();
//    //                                if (!notes.ContainsKey(key))
//    //                                {
//    //                                    notes[key] = noteText;
//    //                                    noteCounter++;
//    //                                }
//    //                            }
//    //                            // Маркер сноски в текст не включаем — он уже есть в child.Value ("[1]")
//    //                            // Но добавляем текст ссылки как есть (напр. "[1]")
//    //                            sb.Append(child.Value);
//    //                        }
//    //                        else
//    //                        {
//    //                            // Обычная гиперссылка — берём текст
//    //                            sb.Append(child.Value);
//    //                        }
//    //                        break;
//    //                    }

//    //                case XElement child when child.Name == NsStrong
//    //                                      || child.Name == NsEmphasis:
//    //                    // Форматирование: берём текст рекурсивно
//    //                    ExtractMixed(child, sb, notesLookup, ref notes, ref noteCounter);
//    //                    break;

//    //                case XElement child:
//    //                    // Любой другой вложенный тег — берём plain text
//    //                    ExtractMixed(child, sb, notesLookup, ref notes, ref noteCounter);
//    //                    break;
//    //            }
//    //        }
//    //    }

//    //    private static (TocDocument toc, List<TocChapterEntry> chapters) BuildToc(
//    //        BookMeta meta,
//    //        List<ParsedElement> elements,
//    //        ConversionOptions options)
//    //    {
//    //        if (elements.Count == 0)
//    //        {
//    //            return (new TocDocument
//    //            {
//    //                Meta = meta,
//    //                FullLength = 0,
//    //                Body = [],
//    //                Parts = []
//    //            }, []);
//    //        }

//    //        // Главы: ищем элементы типа "title" — они разграничивают главы
//    //        var chapters = BuildChapters(elements);

//    //        // Body
//    //        int firstS = elements[0].GlobalIndex;
//    //        int lastE = elements[^1].GlobalIndex;

//    //        // Заголовок всей книги = первый title или мета
//    //        var bookTitle = elements.FirstOrDefault(e => e.Type == "title")?.Text
//    //            ?? meta.Title
//    //            ?? string.Empty;

//    //        var bodyEntry = new TocBodyEntry
//    //        {
//    //            S = firstS,
//    //            E = lastE,
//    //            T = bookTitle,
//    //            C = chapters
//    //        };

//    //        //Parts (фрагменты)
//    //        var parts = BuildParts(elements, options.TargetPartSize);

//    //        var tocDoc = new TocDocument
//    //        {
//    //            Meta = meta,
//    //            FullLength = CalculateFullLength(elements),
//    //            Body = [bodyEntry],
//    //            Parts = parts
//    //        };

//    //        return (tocDoc, chapters);
//    //    }

//    //    /// <summary>
//    //    /// Определяет главы по элементам типа "title"
//    //    /// Первый "title" начинает главу; следующий начинает новую
//    //    /// Элементы до первого title (предисловие, оглавление) включаются
//    //    /// в отдельную запись без заголовка
//    //    /// </summary>
//    //    private static List<TocChapterEntry> BuildChapters(List<ParsedElement> elements)
//    //    {
//    //        var result = new List<TocChapterEntry>();
//    //        int? chapterStart = null;
//    //        string? chapterTitle = null;

//    //        // Элементы до первого title
//    //        var firstTitleIdx = elements.FindIndex(e => e.Type == "title");

//    //        if (firstTitleIdx > 0)
//    //        {
//    //            // Предисловие / фронтматтер
//    //            result.Add(new TocChapterEntry
//    //            {
//    //                S = elements[0].GlobalIndex,
//    //                E = elements[firstTitleIdx - 1].GlobalIndex
//    //            });
//    //        }

//    //        for (int i = firstTitleIdx < 0 ? 0 : firstTitleIdx; i < elements.Count; i++)
//    //        {
//    //            var el = elements[i];

//    //            if (el.Type == "title")
//    //            {
//    //                // Закрываем предыдущую главу
//    //                if (chapterStart.HasValue && i > 0)
//    //                {
//    //                    result.Add(new TocChapterEntry
//    //                    {
//    //                        S = chapterStart.Value,
//    //                        E = elements[i - 1].GlobalIndex,
//    //                        T = chapterTitle
//    //                    });
//    //                }

//    //                chapterStart = el.GlobalIndex;
//    //                chapterTitle = el.Text;
//    //            }
//    //        }

//    //        // Последняя глава
//    //        if (chapterStart.HasValue)
//    //        {
//    //            result.Add(new TocChapterEntry
//    //            {
//    //                S = chapterStart.Value,
//    //                E = elements[^1].GlobalIndex,
//    //                T = chapterTitle
//    //            });
//    //        }

//    //        return result;
//    //    }

//    //    /// <summary>
//    //    /// Нарезает список элементов на фрагменты по <paramref name="targetSize"/> элементов
//    //    /// Нумерация файлов: 000.js, 001.js, ...
//    //    /// </summary>
//    //    private static List<TocPartEntry> BuildParts(
//    //        List<ParsedElement> elements, int targetSize)
//    //    {
//    //        var parts = new List<TocPartEntry>();
//    //        int partIndex = 0;
//    //        int i = 0;

//    //        while (i < elements.Count)
//    //        {
//    //            int end = Math.Min(i + targetSize - 1, elements.Count - 1);

//    //            var first = elements[i];
//    //            var last = elements[end];

//    //            parts.Add(new TocPartEntry
//    //            {
//    //                S = first.GlobalIndex,
//    //                E = last.GlobalIndex,
//    //                Xps = [first.BodyIndex, first.SectionIndex, first.ElemIndex],
//    //                Xpe = [last.BodyIndex, last.SectionIndex, last.ElemIndex],
//    //                Url = $"{partIndex:D3}.js"
//    //            });

//    //            partIndex++;
//    //            i = end + 1;
//    //        }

//    //        return parts;
//    //    }

//    //    /// <summary>
//    //    /// Суммарная длина текстового контента (в символах) для поля full_length
//    //    /// </summary>
//    //    private static int CalculateFullLength(List<ParsedElement> elements)
//    //        => elements.Sum(e => e.Text?.Length ?? 0);

//    //    /// <summary>
//    //    /// Получает плоский текст из XML-элемента, игнорируя теги,
//    //    /// коллапсируя пробелы
//    //    /// </summary>
//    //    private static string FlattenText(XElement el)
//    //    {
//    //        var raw = string.Concat(el.DescendantNodes()
//    //            .OfType<XText>()
//    //            .Select(t => t.Value));
//    //        return CollapseWhitespace(raw);
//    //    }

//    //    private static readonly Regex WhitespaceRegex =
//    //        new(@"\s+", RegexOptions.Compiled);

//    //    private static string CollapseWhitespace(string s)
//    //        => WhitespaceRegex.Replace(s, " ").Trim();

//    //    /// <summary>
//    //    /// Преобразует внутренний <see cref="ParsedElement"/> в выходной <see cref="PartElement"/>
//    //    /// </summary>
//    //    private static PartElement MapToPartElement(ParsedElement pe)
//    //        => new()
//    //        {
//    //            T = pe.Type,
//    //            Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
//    //            C = pe.Text,
//    //            N = pe.Notes
//    //        };
//    //}

//}
