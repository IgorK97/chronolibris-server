using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Chronolibris.Application.Fb2Converter;
using Chronolibris.Application.Fb2Converter.Interfaces;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace Chronolibris.Infrastructure.DataAccess.Fb2Converter
{
    public class Fb2ConverterXReader : IFb2Converter
    {
        private const string Fb2Ns = "http://www.gribuser.ru/xml/fictionbook/2.0";
        private const string XlinkNs = "http://www.w3.org/1999/xlink";

        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly IStorageService _storage;

        public Fb2ConverterXReader(IStorageService storage)
            => _storage = storage;

        public async Task<ConversionResult> ConvertAsync(
            Stream fb2Stream,
            long? bookId = null,
            ConversionOptions? options = null,
            CancellationToken ct = default)
        {
            options ??= new ConversionOptions();

            // Буферируем поток: нужно два прохода, а исходный поток может быть
            // не seekable (например, HttpResponseStream из MinIO).
            // seekable FileStream или MemoryStream напрямую.
            Stream workStream;
            bool ownStream = false;

            if (fb2Stream.CanSeek)
            {
                workStream = fb2Stream;
            }
            else
            {
                var ms = new MemoryStream();
                await fb2Stream.CopyToAsync(ms, ct);
                ms.Position = 0;
                workStream = ms;
                ownStream = true;
            }

            try
            {
                return await ConvertSeekableAsync(workStream, bookId.ToString(), options, ct);
            }
            finally
            {
                if (ownStream) await workStream.DisposeAsync();
            }
        }


        private async Task<ConversionResult> ConvertSeekableAsync(
            Stream stream, string? bookId, ConversionOptions options, CancellationToken ct)
        {
            // ПРОХОД 1
            stream.Position = 0;
            var (rawMeta, notes, imageMap) =
                await FirstPassAsync(stream, bookId, options, ct);

            var resolvedBookId = bookId ?? rawMeta.Uuid ?? Guid.NewGuid().ToString("D");
            var meta = rawMeta with { Uuid = resolvedBookId, Version = options.FormatVersion };

            // ПРОХОД 2
            stream.Position = 0;
            var (elements, tocDoc) = await SecondPassAsync(
                stream, resolvedBookId, meta, notes, imageMap, options, ct);

            // toc.json
            var tocJson = JsonSerializer.Serialize(tocDoc, JsonOpts);
            var tocBytes = Encoding.UTF8.GetByteCount(tocJson);
            await _storage.SaveChunkAsync(resolvedBookId, "toc.json", tocJson, "toc", ct);

            return new ConversionResult
            {
                BookId = resolvedBookId,
                Meta = meta,
                TotalElements = elements,
                TocFile = new StoredFileInfo
                {
                    BookId = resolvedBookId,
                    FileName = "toc.json",
                    FileType = StoredFileType.Toc,
                    SizeBytes = tocBytes
                },
                PartFiles = tocDoc.Parts.Select(p => new StoredFileInfo
                {
                    BookId = resolvedBookId,
                    FileName = p.Url,
                    FileType = StoredFileType.Part,
                    GlobalStart = p.S,
                    GlobalEnd = p.E,
                    XpStart = p.Xps,
                    XpEnd = p.Xpe,
                    SizeBytes = 0   // уже сохранено в проходе 2; для БД достаточно
                }).ToList(),
                CompletedAt = DateTimeOffset.UtcNow
            };
        }

        // ПРОХОД 1: метаданные + сноски + картинки

        private async Task<(BookMeta meta,
                             Dictionary<string, ParsedNote> notes,
                             Dictionary<string, string> imageMap
                             )>
            FirstPassAsync(Stream stream, string? bookId, ConversionOptions options,
                           CancellationToken ct)
        {
            BookMeta? meta = null;
            var notes = new Dictionary<string, ParsedNote>(StringComparer.Ordinal);
            var imageMap = new Dictionary<string, string>(StringComparer.Ordinal);  // fb2-id → fileName
            

            // Используем временный bookId для именования — потом заменим
            var tempBookId = bookId ?? "__temp__";
            int imageIndex = 1;

            // Контекст состояния парсера прохода 1
            bool inDescription = false;
            bool inNotesBody = false;
            bool inNoteSection = false;
            string currentNoteId = "";
            int noteSectionIdx = 0;
            int noteElemIdx = 0;
            int noteBodyIdx = 0; // порядковый номер notes-body (определяем при встрече)
            int bodyCount = 0;

            using var reader = CreateXmlReader(stream);

            while (await reader.ReadAsync())
            {
                ct.ThrowIfCancellationRequested();

                if (reader.NodeType == XmlNodeType.Element)
                {
                    var localName = reader.LocalName;
                    var ns = reader.NamespaceURI;

                    // description → собираем метаданные
                    if (localName == "description" && ns == Fb2Ns)
                    {
                        inDescription = true;
                        var descXml = await reader.ReadOuterXmlAsync();
                        meta = ParseMetaFromXml(descXml);
                        continue;
                    }

                    // body — считаем номер, ищем notes
                    if (localName == "body" && ns == Fb2Ns)
                    {
                        bodyCount++;
                        var nameAttr = reader.GetAttribute("name");
                        if (nameAttr == "notes")
                        {
                            inNotesBody = true;
                            noteBodyIdx = bodyCount;
                        }
                        continue;
                    }

                    // notes body → section
                    if (inNotesBody && localName == "section" && ns == Fb2Ns)
                    {
                        inNoteSection = true;
                        noteSectionIdx++;
                        noteElemIdx = 0;
                        continue;
                    }

                    // notes body → <p id="nX">
                    if (inNotesBody && inNoteSection && localName == "p" && ns == Fb2Ns)
                    {
                        var id = reader.GetAttribute("id");
                        noteElemIdx++;

                        var pXml = await reader.ReadOuterXmlAsync();
                        var text = ExtractTextFromXmlString(pXml);

                        if (!string.IsNullOrEmpty(id))
                        {
                            currentNoteId = id;
                            notes[id] = new ParsedNote
                            {
                                NoteId = id,
                                Xp = [noteBodyIdx, noteSectionIdx, noteElemIdx],
                                Paragraphs = string.IsNullOrEmpty(text) ? [] : [text]
                            };
                        }
                        else if (!string.IsNullOrEmpty(currentNoteId)
                                 && notes.TryGetValue(currentNoteId, out var existing)
                                 && !string.IsNullOrEmpty(text))
                        {
                            existing.Paragraphs.Add(text);
                        }
                        continue;
                    }

                    // <binary id="..." content-type="...">
                    if (localName == "binary" && ns == Fb2Ns)
                    {
                        var binaryId = reader.GetAttribute("id") ?? "";
                        var contentType = reader.GetAttribute("content-type") ?? "image/jpeg";

                        if (!string.IsNullOrEmpty(binaryId))
                        {
                            var ext = ContentTypeToExtension(contentType);
                            var fileName = $"{imageIndex}{ext}";

                            // Читаем base64-содержимое и стримим в MinIO
                            var base64 = await reader.ReadElementContentAsStringAsync();
                            base64 = base64.Trim();

                            if (!string.IsNullOrEmpty(base64))
                            {
                                try
                                {
                                    var bytes = Convert.FromBase64String(base64);
                                    await _storage.SavePublicBookImageAsync(
                                        tempBookId, fileName, bytes, contentType, ct);

                                    imageMap[binaryId] = fileName;
                                    imageIndex++;
                                }
                                catch (FormatException) { /* повреждённый base64 */ }
                            }
                        }
                        continue;
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.LocalName == "body" && reader.NamespaceURI == Fb2Ns)
                        inNotesBody = false;
                    if (reader.LocalName == "section" && reader.NamespaceURI == Fb2Ns)
                        inNoteSection = false;
                }
            }

            meta ??= new BookMeta
            {
                Created = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                Updated = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                Version = options.FormatVersion
            };

            return (meta, notes, imageMap);
        }

        // ПРОХОД 2: основной body → элементы + фрагменты

        private async Task<(int totalElements, TocDocument toc)> SecondPassAsync(
            Stream stream,
            string bookId,
            BookMeta meta,
            Dictionary<string, ParsedNote> notes,
            Dictionary<string, string> imageMap,
            ConversionOptions options,
            CancellationToken ct)
        {
            // Накапливаем элементы текущего фрагмента
            var currentPart = new List<ParsedElement>();
            var allChapters = new List<TocChapterEntry>();
            var tocParts = new List<TocPartEntry>();

            int globalIdx = 0;   // сквозной счётчик элементов
            int bodyIdx = 0;   // индекс текущего основного body
            int bodyCount = 0;
            int sectionIdx = 0;   // сквозной счётчик секций
            int elemIdx = 0;   // счётчик элементов внутри текущей секции
            int partIndex = 0;   // индекс текущего файла-фрагмента

            int totalElements = 0;
            int fullLength = 0;

            // Стек для отслеживания глубины вложенности секций
            var sectionStack = new Stack<(int SectionIdx, int ElemIdx)>();

            bool inMainBody = false;
            bool inSection = false;

            int? firstPartGlobal = null;
            int[]? firstPartXp = null;

            // Буфер для незавершённого фрагмента
            ParsedElement? lastElement = null;

            using var reader = CreateXmlReader(stream);

            while (await reader.ReadAsync())
            {
                ct.ThrowIfCancellationRequested();

                var nodeType = reader.NodeType;
                var localName = reader.LocalName;
                var nsUri = reader.NamespaceURI;

                if (nodeType == XmlNodeType.Element)
                {
                    // Пропускаем notes-body и description
                    if (localName == "description" && nsUri == Fb2Ns)
                    { await reader.SkipAsync(); continue; }

                    if (localName == "body" && nsUri == Fb2Ns)
                    {
                        bodyCount++;
                        var nameAttr = reader.GetAttribute("name");
                        if (nameAttr == "notes")
                        { await reader.SkipAsync(); continue; }
                        // Это основной body
                        bodyIdx = bodyCount;
                        inMainBody = true;
                        continue;
                    }

                    if (localName == "binary" && nsUri == Fb2Ns)
                    { await reader.SkipAsync(); continue; }

                    if (!inMainBody) continue;

                    // section
                    if (localName == "section" && nsUri == Fb2Ns)
                    {
                        sectionIdx++;
                        sectionStack.Push((sectionIdx, elemIdx));
                        elemIdx = 0;
                        continue;
                    }

                    if(localName == "a" && nsUri == Fb2Ns)
                    {
                        var anchorId = reader.GetAttribute("id");
                        var pageNum = TryParsePageNumber(anchorId);

                        if (pageNum.HasValue)
                        {
                            var mySectionIdx = sectionStack.Count > 0 ? sectionStack.Peek().SectionIdx : 0;
                            elemIdx++;
                            var pe = new ParsedElement
                            {
                                Type = "pn",
                                Content = pageNum,
                                Text = null,
                                BodyIndex = bodyIdx,
                                SectionIndex = mySectionIdx,
                                ElemIndex = elemIdx,
                                GlobalIndex = globalIdx
                            };
                            lastElement = pe;
                            currentPart.Add(pe);
                            globalIdx++;
                            totalElements++;

                            if(firstPartGlobal == null)
                            {
                                firstPartGlobal = pe.GlobalIndex;

                                firstPartXp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex];
                            }

                            if(currentPart.Count>= options.TargetPartSize)
                            {
                                partIndex = await FlushPartAsync(bookId, currentPart,
                                    tocParts, partIndex, ct);
                                currentPart.Clear();
                            }
                        }

                        if (!reader.IsEmptyElement)
                            await reader.SkipAsync();
                        continue;
                    }

                    // Текстовые элементы
                    if (localName is "p" or "title" or "subtitle" or "empty-line"
                        or "image" && nsUri == Fb2Ns)
                    {
                        elemIdx++;
                        var mySectionIdx = sectionStack.Count > 0
                            ? sectionStack.Peek().SectionIdx : 0;

                        ParsedElement? pe = null;

                        if (localName == "empty-line")
                        {
                            pe = new ParsedElement
                            {
                                Type = "br",
                                Content = null,
                                Text = null,
                                BodyIndex = bodyIdx,
                                SectionIndex = mySectionIdx,
                                ElemIndex = elemIdx,
                                GlobalIndex = globalIdx
                            };
                            if (!reader.IsEmptyElement) await reader.SkipAsync();
                        }
                        else if (localName == "image")
                        {
                            var href = (reader.GetAttribute("href", XlinkNs)
                                    ?? reader.GetAttribute("href"))?.TrimStart('#');
                            if (href != null && imageMap.TryGetValue(href, out var imgFile))
                            {
                                pe = new ParsedElement
                                {
                                    Type = "img",
                                    Content = new ImgSegment { Src = imgFile },
                                    Text = null,
                                    BodyIndex = bodyIdx,
                                    SectionIndex = mySectionIdx,
                                    ElemIndex = elemIdx,
                                    GlobalIndex = globalIdx
                                };
                            }
                            if (!reader.IsEmptyElement) await reader.SkipAsync();
                        }
                        else
                        {
                            // p / title / subtitle — читаем внутреннее XML
                            var outerXml = await reader.ReadOuterXmlAsync();
                            var (content, flatText) = ParseMixedXml(outerXml, notes, imageMap);

                            if (content != null || localName != "p")
                            {
                                pe = new ParsedElement
                                {
                                    Type = localName == "empty-line" ? "br" : localName,
                                    Content = content,
                                    Text = flatText,
                                    BodyIndex = bodyIdx,
                                    SectionIndex = mySectionIdx,
                                    ElemIndex = elemIdx,
                                    GlobalIndex = globalIdx
                                };
                            }
                        }

                        if (pe == null) continue;

                        // Обновляем TOC: ищем заголовки глав
                        if (pe.Type == "title" && sectionStack.Count <= 1)
                            UpdateChapters(allChapters, pe, globalIdx);

                        fullLength += pe.Text?.Length ?? 0;
                        lastElement = pe;
                        currentPart.Add(pe);
                        globalIdx++;
                        totalElements++;

                        if (firstPartGlobal == null)
                        {
                            firstPartGlobal = pe.GlobalIndex;
                            firstPartXp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex];
                        }

                        // Сохраняем фрагмент при достижении целевого размера
                        if (currentPart.Count >= options.TargetPartSize)
                        {
                            partIndex = await FlushPartAsync(
                                bookId, currentPart, tocParts, partIndex, ct);
                            currentPart.Clear();
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (localName == "body" && nsUri == Fb2Ns)
                        inMainBody = false;

                    if (localName == "section" && nsUri == Fb2Ns && sectionStack.Count > 0)
                    {
                        var (_, savedElem) = sectionStack.Pop();
                        elemIdx = savedElem;
                    }
                }
            }

            // Сохраняем последний незавершённый фрагмент
            if (currentPart.Count > 0)
                partIndex = await FlushPartAsync(bookId, currentPart, tocParts, partIndex, ct);

            // Закрываем последнюю главу
            if (allChapters.Count > 0 && lastElement != null)
            {
                var last = allChapters[^1];
                allChapters[^1] = new TocChapterEntry
                {
                    S = last.S,
                    E = lastElement.GlobalIndex,
                    T = last.T
                };
            }

            var bookTitle = allChapters.FirstOrDefault(c => c.T != null)?.T
                ?? meta.Title ?? string.Empty;

            var tocDoc = new TocDocument
            {
                Meta = meta,
                FullLength = fullLength,
                Body = totalElements > 0
                    ? [new TocBodyEntry
                    {
                        S = tocParts.FirstOrDefault()?.S ?? 0,
                        E = tocParts.LastOrDefault()?.E  ?? 0,
                        T = bookTitle,
                        C = allChapters
                    }]
                    : [],
                Parts = tocParts
            };

            return (totalElements, tocDoc);
        }

        /// <summary>
        /// Парсит смешанный XML-фрагмент (outerXml одного &lt;p&gt; / &lt;title&gt;)
        /// в объект Content (string или List&lt;object&gt;) и plain-text.
        /// Использует легковесный XmlReader — не создаёт полный DOM.
        /// </summary>
        private static (object? content, string? flatText) ParseMixedXml(
            string outerXml,
            Dictionary<string, ParsedNote> notes,
            Dictionary<string, string> imageMap)
        {
            var mixed = new List<object>();
            var buf = new StringBuilder();

            void FlushBuf()
            {
                var s = CollapseWhitespace(buf.ToString());
                if (s.Length > 0) mixed.Add(s);
                buf.Clear();
            }

            using var r = XmlReader.Create(
                new StringReader(outerXml),
                new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });

            // Пропускаем корневой тег (p / title / subtitle)
            r.Read();
            int rootDepth = r.Depth;

            while (r.Read())
            {
                if (r.Depth == rootDepth) break; // вышли из корня

                switch (r.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.SignificantWhitespace:
                        buf.Append(r.Value);
                        break;

                    case XmlNodeType.Element:
                        switch (r.LocalName)
                        {
                            case "strong":
                                {
                                    // Читаем текст ДО вызова ReadElementContentAsString,
                                    // иначе позиция курсора уже сдвинется
                                    FlushBuf();
                                    var inner = r.ReadElementContentAsString().Trim();
                                    if (inner.Length > 0) mixed.Add(new StSegment { C = inner });
                                }
                                break;

                            case "emphasis":
                                {
                                    var inner = r.ReadElementContentAsString().Trim();
                                    FlushBuf();
                                    if (inner.Length > 0) mixed.Add(new EmSegment { C = inner });
                                }
                                break;

                            case "a":
                                {
                                    var noteType = r.GetAttribute("type");
                                    var href = (r.GetAttribute("href", XlinkNs)
                                                 ?? r.GetAttribute("href"))?.TrimStart('#');

                                    var anchorId = r.GetAttribute("id");
                                    var pageNumber = TryParsePageNumber(anchorId);
                                    if (pageNumber.HasValue)
                                    {
                                        FlushBuf();
                                        mixed.Add(new PageNumberSegment { Pn = pageNumber.Value });
                                        if (!r.IsEmptyElement) r.Skip();
                                    }

                                    else
                                    {
                                        var label = r.ReadElementContentAsString();

                                        if (noteType == "note" && href != null
                                            && notes.TryGetValue(href, out var note))
                                        {
                                            FlushBuf();
                                            mixed.Add(new NoteSegment
                                            {
                                                C = label,
                                                Xp = note.Xp,
                                                F = new FootnoteContent
                                                {
                                                    Xp = note.Xp,
                                                    C = note.Paragraphs
                                                }
                                            });
                                        }
                                        else
                                        {
                                            buf.Append(label);
                                        }
                                        
                                    }
                                    break;
                                }

                            case "image":
                                {
                                    var href = (r.GetAttribute("href", XlinkNs)
                                             ?? r.GetAttribute("href"))?.TrimStart('#');
                                    if (href != null && imageMap.TryGetValue(href, out var imgFile))
                                    {
                                        FlushBuf();
                                        mixed.Add(new ImgSegment { Src = imgFile });
                                    }
                                    if (!r.IsEmptyElement) r.Skip();
                                    break;
                                }

                            default:
                                // Прочие теги — берём текст
                                buf.Append(r.ReadElementContentAsString());
                                break;
                        }
                        break;
                }
            }

            FlushBuf();

            if (mixed.Count == 0)
                return (null, null);

            // Если всё — строки, склеиваем в одну
            if (mixed.All(x => x is string))
            {
                var plain = string.Concat(mixed.Cast<string>()).Trim();
                return plain.Length > 0 ? (plain, plain) : (null, null);
            }

            var flatText = string.Concat(mixed.OfType<string>()).Trim();
            return (mixed, flatText.NullIfEmpty());
        }

        /// <summary>
        /// Сериализует накопленный фрагмент, сохраняет в хранилище,
        /// добавляет запись в список tocParts.
        /// </summary>
        private async Task<int> FlushPartAsync(
            string bookId,
            List<ParsedElement> part,
            List<TocPartEntry> tocParts,
            int partIndex,
            CancellationToken ct)
        {
            if (part.Count == 0) return partIndex;

            var first = part[0];
            var last = part[^1];
            var fileName = $"{partIndex:D3}.json";

            var items = part.Select(MapToPartElement).ToList();
            var json = JsonSerializer.Serialize(items, JsonOpts);
            var bytes = Encoding.UTF8.GetByteCount(json);

            await _storage.SaveChunkAsync(bookId, fileName, json, "chunk", ct);

            tocParts.Add(new TocPartEntry
            {
                S = first.GlobalIndex,
                E = last.GlobalIndex,
                Xps = [first.BodyIndex, first.SectionIndex, first.ElemIndex],
                Xpe = [last.BodyIndex, last.SectionIndex, last.ElemIndex],
                Url = fileName
            });

            partIndex++;
            return partIndex;
        }

        private static void UpdateChapters(
            List<TocChapterEntry> chapters,
            ParsedElement titleElement,
            int globalIdx)
        {
            // Закрываем предыдущую главу
            if (chapters.Count > 0)
            {
                var prev = chapters[^1];
                chapters[^1] = new TocChapterEntry
                {
                    S = prev.S,
                    E = globalIdx - 1,
                    T = prev.T
                };
            }
            // Открываем новую
            chapters.Add(new TocChapterEntry
            {
                S = globalIdx,
                E = globalIdx,
                T = titleElement.Text
            });
        }

        // Метаданные (парсим outerXml блока description)

        private static BookMeta ParseMetaFromXml(string descXml)
        {
            using var r = XmlReader.Create(new StringReader(descXml),
                new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });

            var authors = new List<AuthorInfo>();
            var sequences = new List<string>();
            string? title = null, lang = null, annotation = null;
            string? yearPublic = null, yearWritten = null, uuid = null;
            var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz");

            string? firstName = null, lastName = null;
            bool inAuthor = false, inAnnotation = false;
            var annBuf = new StringBuilder();

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.LocalName)
                    {
                        case "book-title": title = r.ReadElementContentAsString().Trim(); break;
                        case "lang": lang = r.ReadElementContentAsString().Trim(); break;
                        case "year": yearPublic = r.ReadElementContentAsString().Trim(); break;
                        case "id": uuid = r.ReadElementContentAsString().Trim(); break;
                        case "author": inAuthor = true; firstName = null; lastName = null; break;
                        case "first-name": if (inAuthor) firstName = r.ReadElementContentAsString().Trim(); break;
                        case "last-name": if (inAuthor) lastName = r.ReadElementContentAsString().Trim(); break;
                        case "sequence": var sn = r.GetAttribute("name"); if (sn != null) sequences.Add(sn); break;
                        case "date":
                            yearWritten = r.GetAttribute("value")?.Trim()
                                ?? r.ReadElementContentAsString().Trim();
                            break;
                        case "annotation": inAnnotation = true; break;
                        case "p": if (inAnnotation) { annBuf.Append(r.ReadElementContentAsString()); annBuf.Append(' '); } break;
                    }
                }
                else if (r.NodeType == XmlNodeType.EndElement)
                {
                    if (r.LocalName == "author")
                    {
                        authors.Add(new AuthorInfo { First = firstName, Last = lastName });
                        inAuthor = false;
                    }
                    if (r.LocalName == "annotation") inAnnotation = false;
                }
            }

            annotation = annBuf.Length > 0 ? annBuf.ToString().Trim() : null;

            return new BookMeta
            {
                Title = title,
                Lang = lang,
                Annotation = annotation,
                Authors = authors.Count > 0 ? authors : null,
                Sequences = sequences.Count > 0 ? sequences : null,
                Written = (yearWritten != null || yearPublic != null)
                    ? new WrittenInfo { Date = yearWritten, DatePublic = yearPublic }
                    : null,
                Uuid = uuid,
                Created = now,
                Updated = now,
                Version = "1.1"
            };
        }

        // Helpers

        private static int? TryParsePageNumber(string? id)
        {
            if (id is null || id.Length < 2|| id[0]!='p') return null;
            return int.TryParse(id.AsSpan(1), out var n) && n > 0 ? n : null;

        }

        private static XmlReader CreateXmlReader(Stream stream)
            => XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreWhitespace = false,
                Async = true
            });

        private static string ExtractTextFromXmlString(string xml)
        {
            try
            {
                using var r = XmlReader.Create(new StringReader(xml),
                    new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });
                var sb = new StringBuilder();
                while (r.Read())
                    if (r.NodeType is XmlNodeType.Text or XmlNodeType.SignificantWhitespace)
                        sb.Append(r.Value);
                return CollapseWhitespace(sb.ToString());
            }
            catch { return string.Empty; }
        }

        //private static PartElement MapToPartElement(ParsedElement pe)
        //{
        //    object? c = pe.Content;

        //    // Если Content — одиночный ImgSegment, передаём как есть (список из одного)
        //    // PartElementJsonConverter умеет сериализовать List<object> и string
        //    if (c is ImgSegment img)
        //        c = new List<object> { img };

        //    return new PartElement
        //    {
        //        T = pe.Type,
        //        Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
        //        C = c
        //    };
        //}

        private static PartElement MapToPartElement(ParsedElement pe)
        {
            object? c = pe.Content;

            // pn-элемент: Content — int (номер страницы).
            // Обходим PartElementJsonConverter передавая int напрямую через JsonSerializer.Serialize.
            if (pe.Type == "pn" && c is int pageNum)
            {
                return new PartElement
                {
                    T = pe.Type,
                    Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
                    C = pageNum   // int сериализуется как JSON number
                };
            }

            // Если Content — одиночный ImgSegment, передаём как список из одного.
            // PartElementJsonConverter умеет сериализовать List<object> и string.
            if (c is ImgSegment img)
                c = new List<object> { img };

            return new PartElement
            {
                T = pe.Type,
                Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
                C = c
            };
        }

        private static string ContentTypeToExtension(string contentType) =>
            contentType.ToLowerInvariant() switch
            {
                "image/png" => ".png",
                _ => ".jpg"
            };

        private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

        private static string CollapseWhitespace(string s)
            => WhitespaceRegex.Replace(s, " ").Trim();
    }

    public sealed class ParsedNote
    {
        public required string NoteId { get; init; }
        /// <summary>[notesBodyIdx, sectionIdx, elemIdx] — 1-based.</summary>
        public required int[] Xp { get; init; }
        /// <summary>Параграфы тела сноски.</summary>
        /// <summary>Параграфы тела сноски — plain strings.</summary>
        public required List<string> Paragraphs { get; init; }
    }

    public static class StringExtensions
    {
        public static string? NullIfEmpty(this string s)
            => string.IsNullOrEmpty(s) ? null : s;
    }

    public class PageNumberSegment
    {
        [JsonPropertyName("pn")]
        public int Pn { get; set; }
    }
}
