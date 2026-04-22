//using System.Text;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;using System.Xml;
//using Chronolibris.Application.Interfaces;
//using Chronolibris.Domain.Interfaces.Services;
//using Chronolibris.Domain.Models;

//namespace Chronolibris.Infrastructure.Services.Fb2Converter
//{
//    public class Fb2ConverterXReader : IFb2Converter
//    {
//        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions()
//        {
//            WriteIndented = true, //отступы
//            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, //не записывать поля со значением null
//            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping //допускание записи специальных символов в их исходном виде без unicode-послежовательностей типа \u003c
//            //читабельность, уменьшение размера. Проблем со скриптами точно НЕ будет!!! React потому что
//        };

//        private readonly IStorageService _storage;

//        public Fb2ConverterXReader(IStorageService storage)
//            => _storage = storage;

//        public async Task<ConversionResult> ConvertAsync(
//            Stream fb2Stream,
//            long bookId,
//            ConversionOptions? options = null,
//            CancellationToken ct = default)
//        {
//            options ??= new ConversionOptions();
//            return await ConvertSeekableAsync(fb2Stream, bookId.ToString(), options, ct);

//            //Алгоритм двупроходный, поэтому поток должен быть таким, чтобы
//            //можно было дважды по нему пройтись.
//            //на всякий случай проверка, вдруг httpResponseStream
//            //Stream workStream;
//            //bool ownStream = false;
//            //options ??= new ConversionOptions();

//            //if (fb2Stream.CanSeek)
//            //{
//            //    workStream = fb2Stream;
//            //}
//            //else
//            //{
//            //    var ms = new MemoryStream();
//            //    await fb2Stream.CopyToAsync(ms, ct);
//            //    ms.Position = 0;
//            //    workStream = ms;
//            //    ownStream = true;
//            //}

//            //try
//            //{
//            //    return await ConvertSeekableAsync(workStream, bookId.ToString(), options, ct);
//            //}
//            //finally
//            //{
//            //    if (ownStream) await workStream.DisposeAsync();
//            //}
//        }


//        private async Task<ConversionResult> ConvertSeekableAsync(
//            Stream stream, string bookId, ConversionOptions options, CancellationToken ct)
//        {
//            //первый проход - метаданные, сбор сносок (они в конце файла, но
//            //ссылки в середине) и ссылки на картинки, получаю все это
//            //деконструкцией кортежа
//            stream.Position = 0;
//            var (rawMeta, notes, imageMap) =
//                await FirstPassAsync(stream, bookId, options, ct);

//            if (rawMeta == null)
//            {
//                rawMeta = new BookMeta
//                {
//                    Id = bookId
//                };
//            }

//            else rawMeta.Id = bookId;

//            //второй проход - сами фрагменты и содержание
//            stream.Position = 0;
//            var (elements, tocDoc) = await SecondPassAsync(
//                stream, bookId, rawMeta, notes, imageMap, options, ct);

//            //Сериализация в JSON содержания и его сохранение
//            var tocJson = JsonSerializer.Serialize(tocDoc, JsonOpts);
//            var tocBytes = Encoding.UTF8.GetByteCount(tocJson);
//            await _storage.SaveChunkAsync(bookId, "toc.json", tocJson, true, ct);

//            //Возвращение результатов конвертации
//            //(в данном сервисе зависимость только от сервиса файлов,
//            //в бд пусть сам хендлер сохраняет)
//            return new ConversionResult
//            {
//                BookId = bookId,
//                TotalElements = elements,
//                TocFile = new StoredFileInfo
//                {
//                    BookId = bookId,
//                    FileName = "toc.json",
//                    FileType = StoredFileType.Toc,
//                    SizeBytes = tocBytes
//                },
//                PartFiles = tocDoc.Parts.Select(p => new StoredFileInfo
//                {
//                    BookId = bookId,
//                    FileName = p.Url,
//                    FileType = StoredFileType.Part,
//                    GlobalStart = p.S,
//                    GlobalEnd = p.E,
//                    XpStart = p.Xps,
//                    XpEnd = p.Xpe,
//                }).ToList(),
//                CompletedAt = DateTime.UtcNow
//            };
//        }

//        //Метод первого прохода
//        private async Task<(BookMeta? meta,
//                             Dictionary<string, ParsedNote> notes,
//                             Dictionary<string, string> imageMap
//                             )>
//            FirstPassAsync(Stream stream, string bookId, ConversionOptions options,
//                           CancellationToken ct)
//        {
//            BookMeta? meta = null;
//            //идентификатор и значение (объект сноски или имя файла в хранилище)
//            var notes = new Dictionary<string, ParsedNote>(StringComparer.Ordinal); //сравнение по кодам символов посимвольно
//            var imageMap = new Dictionary<string, string>(StringComparer.Ordinal);            

//            var tempBookId = bookId;
//            int imageIndex = 1; //для уникальных имен в рамках данного файла книги

//            bool inDescription = false; //для шапки книги с метаданными (пока только название считываю)
//            bool inNotesBody = false; //Fb2 содержит два боди - обычный и body name=notes
//            bool inNoteSection = false; //внутри боди со сносками для каждой сноски обычно используется секция
//            string currentNoteId = ""; //идентификатор абзаца сноски, найденной в тексте
//            int noteSectionIdx = 0; //идентификатор секции скноски
//            int noteElemIdx = 0; //счетчик параграфов внутри секции
//            int noteBodyIdx = 0; //счетчик для боди
//            int bodyCount = 0; //счетчик для боди

//            using var reader = CreateXmlReader(stream); //последовательное чтение потока

//            while (await reader.ReadAsync())
//            {
//                ct.ThrowIfCancellationRequested();

//                if (reader.NodeType == XmlNodeType.Element)
//                {
//                    var localName = reader.LocalName;
//                    //var ns = reader.NamespaceURI;

//                    // в description могут быть нужные метаданные
//                    if (localName == "description")
//                    {
//                        inDescription = true;
//                        var descXml = await reader.ReadOuterXmlAsync();
//                        meta = ParseMetaFromXml(descXml);
//                        continue;
//                    }

//                    // боди для контента и боди для сносок может быть
//                    //пока не нужно считывать контент, поэтому континуе
//                    if (localName == "body")
//                    {
//                        bodyCount++;
//                        var nameAttr = reader.GetAttribute("name");
//                        if (nameAttr == "notes")
//                        {
//                            inNotesBody = true;
//                            noteBodyIdx = bodyCount;
//                        }
//                        continue;
//                    }

//                    //если в боди сносок и вошли в секцию для конкретной сноски
//                    if (inNotesBody && localName == "section")
//                    {
//                        inNoteSection = true;
//                        noteSectionIdx++;
//                        noteElemIdx = 0; //новая секция - новая нумерация параграфов
//                        //(хотя может быть всего один)
//                        continue;
//                    }

//                    //параграф в сноске <p id="nX">
//                    if (inNotesBody && inNoteSection && localName == "p")
//                    {
//                        var id = reader.GetAttribute("id");
//                        noteElemIdx++;

//                        //извлечение содержимого тега и текста из тега
//                        var pXml = await reader.ReadOuterXmlAsync();
//                        var text = ExtractTextFromXmlString(pXml);

//                        if (!string.IsNullOrEmpty(id)) //id будет у первого параграфа только
//                        {
//                            currentNoteId = id;
//                            notes[id] = new ParsedNote //поэтому запись в словаре создается при первом обнаружении
//                            {
//                                NoteId = id,
//                                Xp = [noteBodyIdx, noteSectionIdx, noteElemIdx], //пока трех уровней, надеюсь, будет достаточно
//                                Paragraphs = string.IsNullOrEmpty(text) ? [] : [text]
//                            };
//                        }
//                        //если это уже не первый абзац
//                        else if (!string.IsNullOrEmpty(currentNoteId)
//                                 //&& notes.TryGetValue(currentNoteId, out var existing)
//                                 && !string.IsNullOrEmpty(text))
//                        {
//                            notes[currentNoteId].Paragraphs.Add(text); 
//                        }
//                        continue;
//                    }

//                    // картинка! <binary id="..." content-type="...">
//                    if (localName == "binary")
//                    {
//                        var binaryId = reader.GetAttribute("id") ?? "";
//                        var contentType = reader.GetAttribute("content-type") ?? "image/jpeg";

//                        //только валидную запись смотрю
//                        if (!string.IsNullOrEmpty(binaryId))
//                        {
//                            var ext = ContentTypeToExtension(contentType);
//                            var fileName = $"{imageIndex}{ext}";

//                            //чтение строки картинки
//                            var base64 = await reader.ReadElementContentAsStringAsync();
//                            base64 = base64.Trim();

//                            if (!string.IsNullOrEmpty(base64))
//                            {
//                                try
//                                {
//                                    var bytes = Convert.FromBase64String(base64); //строку в массив байт,
//                                    //а потом обернуть в поток и сохранить в хранилище

//                                    using (var coverStream = new MemoryStream(bytes))
//                                    {
//                                        await _storage.SaveImageAsync(
//                                            tempBookId, fileName, coverStream, contentType, ct);
//                                    }

//                                    imageMap[binaryId] = fileName;
//                                    imageIndex++;
//                                }
//                                catch (FormatException) { /* повреждённый base64 */ } //можно и прервать,
//                                //но картинки того не стоит (но как тогда уведомить админа?)
//                                //потом можно добавить строку в класс результата и, если она не пустая,
//                                //то вместе с ответом и ее возвращать тоже
//                            }
//                        }
//                        continue;
//                    }
//                }

//                if (reader.NodeType == XmlNodeType.EndElement)
//                {
//                    if (reader.LocalName == "body")
//                        inNotesBody = false;
//                    if (reader.LocalName == "section")
//                        inNoteSection = false;
//                }
//            }

//            return (meta, notes, imageMap);
//        }

//        //метод второго прохода

//        private async Task<(int totalElements, TocDocument toc)> SecondPassAsync(
//            Stream stream,
//            string bookId,
//            BookMeta meta,
//            Dictionary<string, ParsedNote> notes,
//            Dictionary<string, string> imageMap,
//            ConversionOptions options,
//            CancellationToken ct)
//        {
//            //список элементов текущего накапливаемого фрагмента
//            var currentPart = new List<ParsedElement>();
//            //списки метаданных фрагментов
//            var allChapters = new List<TocChapter>(); //список всех глав для оглавления
//            var tocParts = new List<TocPart>(); //список всех сохраненных фрагментов для TOC

//            int globalIdx = 0;   //сквозной индекс элемента по всей книге
//            int bodyIdx = 0;   //порядковый номер текущего основного боди
//            int bodyCount = 0; //количество боди
//            int sectionIdx = 0;   //сквозной счетчик секций
//            int elemIdx = 0;   // счетчик элементов внутри текущей секции
//            int partIndex = 0;   // индекс текущего файла-фрагмента

//            int totalElements = 0; //количество всех фрагментов
//            int fullLength = 0; //длина книги

//            //стек секций - при входе в секцию сохраняется sectionIdx, elemIdx
//            var sectionStack = new Stack<(int SectionIdx, int ElemIdx)>();

//            bool inMainBody = false; //внутри основного боди
//            //bool inSection = false;

//            int? firstPartGlobal = null; //индекс первого элемента книги для оглавления ток
//            int[]? firstPartXp = null; //xp-иднекс [bodyIdx, sectionIdx, elemIdx] первого элемента книги

//            //Последний фрагмент - чтобы корректно его обработать
//            ParsedElement? lastElement = null;

//            using var reader = CreateXmlReader(stream);

//            while (await reader.ReadAsync())
//            {
//                ct.ThrowIfCancellationRequested();

//                var nodeType = reader.NodeType;
//                var localName = reader.LocalName;
//                //var nsUri = reader.NamespaceURI;

//                if (nodeType == XmlNodeType.Element)
//                {
//                    //пропуск описания - в читалке не нужно
//                    if (localName == "description")
//                    { await reader.SkipAsync(); continue; }

//                    if (localName == "body")
//                    {
//                        bodyCount++;
//                        var nameAttr = reader.GetAttribute("name");
//                        if (nameAttr == "notes") //пропуск сносок (они уже обработаны ранее)
//                        { await reader.SkipAsync(); continue; }
//                        // Это основной боди
//                        bodyIdx = bodyCount;
//                        inMainBody = true;
//                        continue;
//                    }

//                    if (localName == "binary") //картинки тоже уже обработаны
//                    { await reader.SkipAsync(); continue; }

//                    if (!inMainBody) continue; // если иной элемент (типа секции) - но не в главном боди,
//                    //то тоже нужно пропустить - не нужно либо уже обработано

//                    //глава или раздел
//                    if (localName == "section")
//                    {
//                        sectionIdx++;
//                        sectionStack.Push((sectionIdx, elemIdx));
//                        elemIdx = 0; //нумерация внутри секции новой началась
//                        continue;
//                    }

//                    if(localName == "a") //номер страницы (сейчас только номер)
//                    {
//                        var anchorId = reader.GetAttribute("id");
//                        var pageNum = TryParsePageNumber(anchorId); //сам номер страницы
//                        //если номер был указан корректно, то его нужно обработать
//                        if (pageNum.HasValue)
//                        {
//                            var mySectionIdx = sectionStack.Count > 0 ? sectionStack.Peek().SectionIdx : 0;
//                            elemIdx++; //это тоже элемент
//                            var pe = new ParsedElement //новый элемент
//                            {
//                                Type = "pn",
//                                Content = pageNum,
//                                Text = null,
//                                BodyIndex = bodyIdx, //потом, если нужно, можно просто использовать массив индексов
//                                //вместо трех номеров
//                                SectionIndex = mySectionIdx,
//                                ElemIndex = elemIdx,
//                                GlobalIndex = globalIdx
//                            };
//                            lastElement = pe;
//                            currentPart.Add(pe);
//                            globalIdx++;
//                            totalElements++;

//                            if(firstPartGlobal == null)
//                            {
//                                firstPartGlobal = pe.GlobalIndex;

//                                firstPartXp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex];
//                            }
//                            //если накоплено достаточно фрагментов, то нужно их сбросить
//                            //в хранилище и очистить
//                            if(currentPart.Count>= options.TargetPartSize)
//                            {
//                                partIndex = await FlushPartAsync(bookId, currentPart,
//                                    tocParts, partIndex, ct);
//                                currentPart.Clear();
//                            }
//                        }
//                        //содержимое не нужно (даже если есть)
//                        if (!reader.IsEmptyElement)
//                            await reader.SkipAsync();
//                        continue;
//                    }

//                    //разные возможные текстовые элементы  (или image)
//                    if (localName is "p" or "title" or "subtitle" or "empty-line"
//                        or "image")
//                    {
//                        elemIdx++;
//                        var mySectionIdx = sectionStack.Count > 0
//                            ? sectionStack.Peek().SectionIdx : 0;

//                        ParsedElement? pe = null;

//                        if (localName == "empty-line")
//                        {
//                            pe = new ParsedElement
//                            {
//                                Type = "br",
//                                Content = null,
//                                Text = null,
//                                BodyIndex = bodyIdx,
//                                SectionIndex = mySectionIdx,
//                                ElemIndex = elemIdx,
//                                GlobalIndex = globalIdx
//                            };
//                            if (!reader.IsEmptyElement) await reader.SkipAsync();
//                        }
//                        //ссылка на картинку в тексте - сама картинка указывается ссылкой
//                        //в href на тот тег binary, который расположен в конце файла
//                        else if (localName == "image")
//                        {
//                            var href = reader.GetAttribute("href")?.TrimStart('#');
//                            if (href != null && imageMap.TryGetValue(href, out var imgFile))
//                            {
//                                pe = new ParsedElement
//                                {
//                                    Type = "img",
//                                    Content = new ImgSegment { Src = imgFile },
//                                    Text = null,
//                                    BodyIndex = bodyIdx,
//                                    SectionIndex = mySectionIdx,
//                                    ElemIndex = elemIdx,
//                                    GlobalIndex = globalIdx
//                                };
//                            }
//                            if (!reader.IsEmptyElement) await reader.SkipAsync();
//                        }
//                        else
//                        {
//                            //все остальное - уже текстовое
//                            var outerXml = await reader.ReadOuterXmlAsync(); //тег целиком
//                            var (content, flatText) = ParseMixedXml(outerXml, notes, imageMap); //он могу быть смешанным
//                            //(с другими сносками и картинками и т.д.) - в общем, рекурсивная структура гипотетически

//                            if (content != null || localName != "p") //контента может не быть, 
//                                //но это может быть заголовок и т.д. - в таком случае, тоже сохранить (потом можно проверить
//                                //и дополнить, если что)
//                            {
//                                pe = new ParsedElement
//                                {
//                                    Type = localName,
//                                    Content = content, //сегменты текста, ссылки и т.д.
//                                    Text = flatText, //мог быть и текст просто
//                                    BodyIndex = bodyIdx,
//                                    SectionIndex = mySectionIdx,
//                                    ElemIndex = elemIdx,
//                                    GlobalIndex = globalIdx
//                                };
//                            }
//                        }

//                        if (pe == null) continue; //непонятно какой тег - пропустить

//                        //Заголовок верхнего уровня (это стек,
//                        //если там до одного элемента, значит, заголовок верхнего уровня)
//                        //Если потребуется составить оглавление многоуровневое - 
//                        //подправить потом здесь
//                        if (pe.Type == "title" && sectionStack.Count <= 1)
//                            UpdateChapters(allChapters, pe, globalIdx);

//                        fullLength += pe.Text?.Length ?? 0; //добавление длины текста
//                        lastElement = pe;
//                        currentPart.Add(pe);
//                        globalIdx++;
//                        totalElements++;

//                        if (firstPartGlobal == null)
//                        {
//                            firstPartGlobal = pe.GlobalIndex;
//                            firstPartXp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex];
//                        }

//                        //Накоплено достаточно - сбросить и очистить
//                        if (currentPart.Count >= options.TargetPartSize)
//                        {
//                            partIndex = await FlushPartAsync(
//                                bookId, currentPart, tocParts, partIndex, ct);
//                            currentPart.Clear();
//                        }
//                    }
//                }
//                else if (nodeType == XmlNodeType.EndElement)
//                {
//                    if (localName == "body")
//                        inMainBody = false;

//                    //если это была секция, то нужно восстановить последний
//                    //номер элемента уровня секции
//                    if (localName == "section" && sectionStack.Count > 0)
//                    {
//                        var (_, savedElem) = sectionStack.Pop(); //используется дискард (выброс0 - мне он не нужен
//                        elemIdx = savedElem;
//                    }
//                }
//            }

//            //Если не сбросили содержимое послежнего фрагмента,
//            //при этом это содержимое есть - то сброс принудительно
//            if (currentPart.Count > 0)
//                partIndex = await FlushPartAsync(bookId, currentPart, tocParts, partIndex, ct);

//            //При этом нужно не забыть про главу
//            if (allChapters.Count > 0 && lastElement != null)
//            {
//                //var last = allChapters[^1]; //индекс от конца - самый послежний элемент
//                //allChapters[^1] = new TocChapter
//                //{
//                //    S = last.S,
//                //    E = lastElement.GlobalIndex,
//                //    T = last.T
//                //};
//                allChapters[^1].E = lastElement.GlobalIndex; //раньше был тип record, 
//                //зачем мне он нужен вообще? Пусть класс будет!
//            }

//            var bookTitle = meta.Title ?? allChapters.FirstOrDefault(c => c.T != null)?.T //title
//                ?? string.Empty; //заголовок книги - алгоритм мог найти его еще
//            //в секции описания, либо просто взять самую первую секцию из боди

//            var tocDoc = new TocDocument
//            {
//                Meta = meta,
//                FullLength = fullLength, //длина текста целиком
//                Body = totalElements > 0
//                    ? [new TocBody
//                    {
//                        S = tocParts.FirstOrDefault()?.S ?? 0,
//                        E = tocParts.LastOrDefault()?.E  ?? 0,
//                        T = bookTitle,
//                        C = allChapters
//                    }]
//                    : [], //в файле fb2 один боди, но потом можно либо сохранять просто фрагменты,
//                //либо несколько боди - если будет иной формат, типа fb3 или epub (там их уже несколько)
//                //кроме того, здесь сохраняются главы-чаптеры (ААА, нужно потом дополнить!!!)
//                Parts = tocParts //список фрагментов нужен обязательно - для быстрого перехода по оглавлению к нужному фрагменту
//            };

//            return (totalElements, tocDoc);
//        }

//        // Парсит смешанный XML-фрагмент
//        // в объект Content (string или List или object) и plain-text
//        // Использует легковесный XmlReader — не создаёт полный DOM
//        private static (object? content, string? flatText) ParseMixedXml(
//            string outerXml,
//            Dictionary<string, ParsedNote> notes,
//            Dictionary<string, string> imageMap)
//        {
//            var mixed = new List<object>(); //это может быть большой список из разного содержимого, по идее
//            var buf = new StringBuilder();

//            void FlushBuf() //сбросить накопленную строку
//            {
//                var s = CollapseWhitespace(buf.ToString()); //удалить лишние пробелы
//                if (s.Length > 0) mixed.Add(s);
//                buf.Clear();
//            }

//            using var r = XmlReader.Create( //напрямую в констурктор, как оказалось, не принимает строку, поэтому использовал stringReader
//                new StringReader(outerXml),
//                new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }); //по умолчанию prohibit

//            //Здесь корневой тег, его нужно пропустить
//            r.Read();
//            int rootDepth = r.Depth; //абсолютная глубина

//            while (r.Read())
//            {
//                if (r.Depth == rootDepth) break; //на всякий случай выход сразу по окончании корневого тега

//                switch (r.NodeType)
//                {
//                    case XmlNodeType.Text:
//                    case XmlNodeType.SignificantWhitespace: //например, в теге p с xml:space=preserve пробелы с переносами или иным содержимым могут стать именно этим
//                        buf.Append(r.Value);
//                        break;

//                    case XmlNodeType.Element:
//                        switch (r.LocalName)
//                        {
//                            case "strong":
//                                {

//                                    FlushBuf(); //сброс уже накопленного текста
//                                    var inner = r.ReadElementContentAsString().Trim();
//                                    if (inner.Length > 0) mixed.Add(new StSegment { C = inner }); //сегмент жирного текста
//                                }
//                                break;

//                            case "emphasis": //курсив - аналогично
//                                {
//                                    FlushBuf();
//                                    var inner = r.ReadElementContentAsString().Trim();
//                                    if (inner.Length > 0) mixed.Add(new EmSegment { C = inner });
//                                }
//                                break;

//                            case "a": //пока только якоря (для номеров страниц) или ссылки для сносок
//                                {
//                                    var noteType = r.GetAttribute("type");
//                                    var href = r.GetAttribute("href")?.TrimStart('#');

//                                    var anchorId = r.GetAttribute("id");
//                                    var pageNumber = TryParsePageNumber(anchorId);
//                                    if (pageNumber.HasValue)
//                                    {
//                                        FlushBuf();
//                                        mixed.Add(new PageNumberSegment { Pn = pageNumber.Value });
//                                        if (!r.IsEmptyElement) r.Skip();
//                                    }

//                                    else
//                                    {
//                                        var label = r.ReadElementContentAsString();

//                                        if (noteType == "note" && href != null
//                                            && notes.TryGetValue(href, out var note))
//                                        {
//                                            FlushBuf();
//                                            mixed.Add(new NoteSegment
//                                            {
//                                                C = label,
//                                                Xp = note.Xp,
//                                                F = new FootnoteContent
//                                                {
//                                                    Xp = note.Xp,
//                                                    C = note.Paragraphs
//                                                }
//                                            });
//                                        }
//                                        else
//                                        {
//                                            buf.Append(label);
//                                        }

//                                    }
//                                    break;
//                                }

//                            case "image":
//                                {
//                                    var href = r.GetAttribute("href")?.TrimStart('#');
//                                    if (href != null && imageMap.TryGetValue(href, out var imgFile))
//                                    {
//                                        FlushBuf();
//                                        mixed.Add(new ImgSegment { Src = imgFile });
//                                    }
//                                    if (!r.IsEmptyElement) r.Skip();
//                                    break;
//                                }

//                            default:
//                                //прочие теги - только текст
//                                buf.Append(r.ReadElementContentAsString());
//                                break;
//                        }
//                        break;
//                }
//            }

//            FlushBuf();

//            if (mixed.Count == 0)
//                return (null, null);

//            //если все строки, то можно склеить в одну
//            if (mixed.All(x => x is string))
//            {
//                var plain = string.Concat(mixed.Cast<string>()).Trim(); //приведение типа к строке
//                return plain.Length > 0 ? (plain, plain) : (null, null);
//            }

//            var flatText = string.Concat(mixed.OfType<string>()).Trim();
//            var s = string.IsNullOrEmpty(flatText) ? null : flatText; //если все строки были только пробелами
//            return (mixed, s);
//        }

//        // Сериализует накопленный фрагмент, сохраняет в хранилище,
//        // добавляет запись в список tocParts
//        private async Task<int> FlushPartAsync(
//            string bookId,
//            List<ParsedElement> part,
//            List<TocPart> tocParts,
//            int partIndex,
//            CancellationToken ct)
//        {
//            if (part.Count == 0) return partIndex; //проверка на пустой фрагмент - изменений нет

//            var first = part[0];
//            var last = part[^1];
//            var fileName = $"{partIndex:D3}.json"; //000.json,..., строка формата, целое число, 3 цифры

//            var items = part.Select(MapToPartElement).ToList(); //преобразовать ParsedElement в PartElement для сохранения, группа методов или делегат (стрелочная функция)
//            var json = JsonSerializer.Serialize(items, JsonOpts);
//            var bytes = Encoding.UTF8.GetByteCount(json);

//            await _storage.SaveChunkAsync(bookId, fileName, json, false, ct);

//            tocParts.Add(new TocPart
//            {
//                S = first.GlobalIndex,
//                E = last.GlobalIndex,
//                Xps = [first.BodyIndex, first.SectionIndex, first.ElemIndex],
//                Xpe = [last.BodyIndex, last.SectionIndex, last.ElemIndex],
//                Url = fileName
//            });

//            partIndex++;
//            return partIndex;
//        }

//        //обновляет список глав
//        private static void UpdateChapters(
//            List<TocChapter> chapters,
//            ParsedElement titleElement,
//            int globalIdx)
//        {
//            //закрытие предыдущей главы
//            if (chapters.Count > 0)
//            {
//                //var prev = chapters[^1];
//                //chapters[^1] = new TocChapter
//                //{
//                //    S = prev.S,
//                //    E = globalIdx - 1,
//                //    T = prev.T
//                //};
//                chapters[^1].E = globalIdx - 1;
//            }
//            //открытие новой
//            chapters.Add(new TocChapter
//            {
//                S = globalIdx,
//                E = globalIdx,
//                T = titleElement.Text
//            });
//        }

//        // Метаданные (название книги) (парсинг outerXml блока description)

//        private static BookMeta ParseMetaFromXml(string descXml)
//        {
//            using var r = XmlReader.Create(new StringReader(descXml),
//                new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });


//            string? title = null;
//            while (r.Read())
//            {
//                if (r.NodeType == XmlNodeType.Element)
//                {
//                    switch (r.LocalName)
//                    {
//                        case "book-title": title = r.ReadElementContentAsString().Trim(); break;
//                    }
//                }
//            }
//            return new BookMeta
//            {
//                Title = title,
//            };
//        }

//        //парсинг номера страницы - обязательно как pN
//        private static int? TryParsePageNumber(string? id)
//        {
//            if (id is null || id.Length < 2|| id[0]!='p') return null;
//            //взять строку без первого символа без аллокации
//            return int.TryParse(id.AsSpan(1), out var n) && n > 0 ? n : null;

//        }

//        private static XmlReader CreateXmlReader(Stream stream)
//            => XmlReader.Create(stream, new XmlReaderSettings
//            {
//                DtdProcessing = DtdProcessing.Ignore,
//                IgnoreWhitespace = false, //сохранение пробельных узлов
//                Async = true //поддержка асинк методов
//            });

//        private static string ExtractTextFromXmlString(string xml)
//        {
//            try
//            {
//                using var r = XmlReader.Create(new StringReader(xml),
//                    new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });
//                var sb = new StringBuilder();
//                while (r.Read())
//                    if (r.NodeType is XmlNodeType.Text or XmlNodeType.SignificantWhitespace)
//                        sb.Append(r.Value);
//                return CollapseWhitespace(sb.ToString()); //убрать лишние пробелы
//            }
//            catch { return string.Empty; }
//        }

//        //преобразование предварительной модели в конечную
//        private static PartElement MapToPartElement(ParsedElement pe)
//        {
//            object? c = pe.Content;

//            // pn-элемент: Content — int (номер страницы)
//            if (pe.Type == "pn" && c is int pageNum)
//            {
//                return new PartElement
//                {
//                    T = pe.Type,
//                    Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
//                    C = pageNum
//                };
//            }

//            // Если Content — одиночный ImgSegment, передаём как список из одного
//            if (c is ImgSegment img)
//                c = new List<object> { img };

//            return new PartElement
//            {
//                T = pe.Type,
//                Xp = [pe.BodyIndex, pe.SectionIndex, pe.ElemIndex],
//                C = c
//            };
//        }

//        private static string ContentTypeToExtension(string contentType) =>
//            contentType.ToLowerInvariant() switch
//            {
//                "image/png" => ".png",
//                _ => ".jpg"
//            };

//        private static readonly Regex WhitespaceRegex = new Regex(@"\s+"); //регулярное выражение
//        //верабльный литерал (игнорирование обратных слешей, чтобы не было \\)
//        //пробел, переносы и табуляции

//        private static string CollapseWhitespace(string s)
//            => WhitespaceRegex.Replace(s, " ").Trim();
//    }
//}


using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Chronolibris.Application.Interfaces;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Chronolibris.Infrastructure.Services.Fb2Converter
{
    public class Fb2ConverterXReader : IFb2Converter
    {
        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions()
        {
            WriteIndented = true, //отступы
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, //не записывать поля со значением null
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping //допускание записи специальных символов в их исходном виде без unicode-послежовательностей типа \u003c
            //читабельность, уменьшение размера. Проблем со скриптами точно НЕ будет!!! React потому что
        };

        private readonly IStorageService _storage;
        private readonly ILogger<Fb2ConverterXReader> _logger;

        public Fb2ConverterXReader(IStorageService storage, ILogger<Fb2ConverterXReader> logger)
        {
            _storage = storage;
            _logger = logger;        
        }

        public async Task<ConversionResult> ConvertAsync(
            Stream fb2Stream,
            long bookId,
            ConversionOptions? options = null,
            CancellationToken ct = default)
        {
            options ??= new ConversionOptions();
            return await ConvertSeekableAsync(fb2Stream, bookId.ToString(), options, ct);

            //Алгоритм двупроходный, поэтому поток должен быть таким, чтобы
            //можно было дважды по нему пройтись.
            //на всякий случай проверка, вдруг httpResponseStream
            //Stream workStream;
            //bool ownStream = false;
            //options ??= new ConversionOptions();

            //if (fb2Stream.CanSeek)
            //{
            //    workStream = fb2Stream;
            //}
            //else
            //{
            //    var ms = new MemoryStream();
            //    await fb2Stream.CopyToAsync(ms, ct);
            //    ms.Position = 0;
            //    workStream = ms;
            //    ownStream = true;
            //}

            //try
            //{
            //    return await ConvertSeekableAsync(workStream, bookId.ToString(), options, ct);
            //}
            //finally
            //{
            //    if (ownStream) await workStream.DisposeAsync();
            //}
        }


        private async Task<ConversionResult> ConvertSeekableAsync(
            Stream stream, string bookId, ConversionOptions options, CancellationToken ct)
        {
            //первый проход - метаданные, сбор сносок (они в конце файла, но
            //ссылки в середине) и ссылки на картинки, получаю все это
            //деконструкцией кортежа
            stream.Position = 0;
            var (rawMeta, notes, imageMap) =
                await FirstPassAsync(stream, bookId, options, ct);

            if (rawMeta == null)
            {
                rawMeta = new BookMeta
                {
                    Id = bookId
                };
            }

            else rawMeta.Id = bookId;

            //второй проход - сами фрагменты и содержание
            stream.Position = 0;
            var (elements, tocDoc) = await SecondPassAsync(
                stream, bookId, rawMeta, notes, imageMap, options, ct);

            //Сериализация в JSON содержания и его сохранение
            var tocJson = JsonSerializer.Serialize(tocDoc, JsonOpts);
            var tocBytes = Encoding.UTF8.GetByteCount(tocJson);
            await _storage.SaveChunkAsync(bookId, "toc.json", tocJson, true, ct);

            //Возвращение результатов конвертации
            //(в данном сервисе зависимость только от сервиса файлов,
            //в бд пусть сам хендлер сохраняет)
            return new ConversionResult
            {
                BookId = bookId,
                TotalElements = elements,
                TocFile = new StoredFileInfo
                {
                    BookId = bookId,
                    FileName = "toc.json",
                    FileType = StoredFileType.Toc,
                    SizeBytes = tocBytes
                },
                PartFiles = tocDoc.Parts.Select(p => new StoredFileInfo
                {
                    BookId = bookId,
                    FileName = p.Url,
                    FileType = StoredFileType.Part,
                    GlobalStart = p.S,
                    GlobalEnd = p.E,
                    XpStart = p.Xps,
                    XpEnd = p.Xpe,
                }).ToList(),
                CompletedAt = DateTime.UtcNow
            };
        }

        //Метод первого прохода
        private async Task<(BookMeta? meta,
                             Dictionary<string, ParsedNote> notes,
                             Dictionary<string, string> imageMap
                             )>
            FirstPassAsync(Stream stream, string bookId, ConversionOptions options,
                           CancellationToken ct)
        {
            BookMeta? meta = null;
            //идентификатор и значение (объект сноски или имя файла в хранилище)
            var notes = new Dictionary<string, ParsedNote>(StringComparer.Ordinal); //сравнение по кодам символов посимвольно
            var imageMap = new Dictionary<string, string>(StringComparer.Ordinal);

            var tempBookId = bookId;
            int imageIndex = 1; //для уникальных имен в рамках данного файла книги

            bool inDescription = false; //для шапки книги с метаданными (пока только название считываю)
            bool inNotesBody = false; //Fb2 содержит два боди - обычный и body name=notes
            bool inNoteSection = false; //внутри боди со сносками для каждой сноски обычно используется секция
            string currentNoteId = ""; //идентификатор абзаца сноски, найденной в тексте
            int noteSectionIdx = 0; //идентификатор секции скноски
            int noteElemIdx = 0; //счетчик параграфов внутри секции
            int noteBodyIdx = 0; //счетчик для боди
            int bodyCount = 0; //счетчик для боди

            using var reader = CreateXmlReader(stream); //последовательное чтение потока

            while (await reader.ReadAsync())
            {
                ct.ThrowIfCancellationRequested();
                _logger.LogDebug("type {Type}--->node with name {Name}, with value {Value}",
                    reader.NodeType, reader.LocalName, reader.Value);
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var localName = reader.LocalName;
                    //var ns = reader.NamespaceURI;

                    // в description могут быть нужные метаданные
                    if (localName == "description")
                    {
                        inDescription = true;
                        var descXml = await reader.ReadOuterXmlAsync();
                        meta = ParseMetaFromXml(descXml);
                        continue;
                    }

                    // боди для контента и боди для сносок может быть
                    //пока не нужно считывать контент, поэтому континуе
                    if (localName == "body")
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

                    //если в боди сносок и вошли в секцию для конкретной сноски
                    if (inNotesBody && localName == "section")
                    {
                        inNoteSection = true;
                        noteSectionIdx++;
                        noteElemIdx = 0; //новая секция - новая нумерация параграфов
                        //(хотя может быть всего один)
                        continue;
                    }

                    //параграф в сноске <p id="nX">
                    if (inNotesBody && inNoteSection && localName == "p")
                    {
                        var id = reader.GetAttribute("id");
                        noteElemIdx++;

                        //извлечение содержимого тега и текста из тега
                        var pXml = await reader.ReadOuterXmlAsync();
                        var text = ExtractTextFromXmlString(pXml);

                        if (!string.IsNullOrEmpty(id)) //id будет у первого параграфа только
                        {
                            currentNoteId = id;
                            notes[id] = new ParsedNote //поэтому запись в словаре создается при первом обнаружении
                            {
                                NoteId = id,
                                Xp = [noteBodyIdx, noteSectionIdx, noteElemIdx], // body notes всегда плоские: body > section > p
                                Paragraphs = string.IsNullOrEmpty(text) ? [] : [text]
                            };
                        }
                        //если это уже не первый абзац
                        else if (!string.IsNullOrEmpty(currentNoteId)
                                 //&& notes.TryGetValue(currentNoteId, out var existing)
                                 && !string.IsNullOrEmpty(text))
                        {
                            notes[currentNoteId].Paragraphs.Add(text);
                        }
                        continue;
                    }

                    // картинка! <binary id="..." content-type="...">
                    if (localName == "binary")
                    {
                        var binaryId = reader.GetAttribute("id") ?? "";
                        var contentType = reader.GetAttribute("content-type") ?? "image/jpeg";

                        //только валидную запись смотрю
                        if (!string.IsNullOrEmpty(binaryId))
                        {
                            var ext = ContentTypeToExtension(contentType);
                            var fileName = $"{imageIndex}{ext}";

                            //чтение строки картинки
                            var base64 = await reader.ReadElementContentAsStringAsync();
                            base64 = base64.Trim();

                            if (!string.IsNullOrEmpty(base64))
                            {
                                try
                                {
                                    var bytes = Convert.FromBase64String(base64); //строку в массив байт,
                                    //а потом обернуть в поток и сохранить в хранилище

                                    using (var coverStream = new MemoryStream(bytes))
                                    {
                                        await _storage.SaveImageAsync(
                                            tempBookId, fileName, coverStream, contentType, ct);
                                    }

                                    imageMap[binaryId] = fileName;
                                    imageIndex++;
                                }
                                catch (FormatException) { /* повреждённый base64 */ } //можно и прервать,
                                //но картинки того не стоит (но как тогда уведомить админа?)
                                //потом можно добавить строку в класс результата и, если она не пустая,
                                //то вместе с ответом и ее возвращать тоже
                            }
                        }
                        continue;
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.LocalName == "body")
                        inNotesBody = false;
                    if (reader.LocalName == "section")
                        inNoteSection = false;
                }
            }

            return (meta, notes, imageMap);
        }

        //метод второго прохода

        private async Task<(int totalElements, TocDocument toc)> SecondPassAsync(
            Stream stream,
            string bookId,
            BookMeta meta,
            Dictionary<string, ParsedNote> notes,
            Dictionary<string, string> imageMap,
            ConversionOptions options,
            CancellationToken ct)
        {
            //список элементов текущего накапливаемого фрагмента
            var currentPart = new List<ParsedElement>();
            //списки метаданных фрагментов
            var allChapters = new List<TocChapter>(); //список всех глав для оглавления
            var tocParts = new List<TocPart>(); //список всех сохраненных фрагментов для TOC

            int globalIdx = 0;   //сквозной индекс элемента по всей книге
            int bodyIdx = 0;   //порядковый номер текущего основного боди
            int bodyCount = 0; //количество боди
            int partIndex = 0;   // индекс текущего файла-фрагмента

            int totalElements = 0; //количество всех фрагментов
            int fullLength = 0; //длина книги

            // Стек секций. Каждый элемент стека соответствует одному уровню вложенности секции.
            // SectionOrdinal — порядковый номер данной секции среди сестёр на своём уровне
            //   (увеличивается при входе в секцию, до того как она попадает в стек).
            // ElemCount — текущий счётчик элементов внутри этой секции.
            // ParentElemCount — значение счётчика элементов родительского уровня,
            //   которое нужно восстановить при выходе из секции.
            var sectionStack = new Stack<(int SectionOrdinal, int ParentElemCount)>();

            // sectionCounters[depth] — сколько секций уже открыто на данной глубине
            // (depth 0 = прямые дочери body). Растёт при каждом входе в секцию, не сбрасывается.
            var sectionCounters = new List<int>();

            // Счётчик элементов текущего уровня (если стек пуст — уровень body)
            int elemIdx = 0;

            bool inMainBody = false; //внутри основного боди
            //bool inSection = false;

            int? firstPartGlobal = null; //индекс первого элемента книги для оглавления ток
            int[]? firstPartXp = null; //xp первого элемента книги

            //Последний фрагмент - чтобы корректно его обработать
            ParsedElement? lastElement = null;
            _logger.LogDebug("===second trip===");
            using var reader = CreateXmlReader(stream);

            while (await reader.ReadAsync())
            {
                ct.ThrowIfCancellationRequested();
                _logger.LogDebug("type {Type}--->node with name {Name}, with value {Value}",
    reader.NodeType, reader.LocalName, reader.Value);
                var nodeType = reader.NodeType;
                var localName = reader.LocalName;
                //var nsUri = reader.NamespaceURI;

                if (nodeType == XmlNodeType.Element)
                {
                    //пропуск описания - в читалке не нужно
                    if (localName == "description")
                    { await reader.SkipAsync(); continue; }

                    if (localName == "body")
                    {
                        bodyCount++;
                        var nameAttr = reader.GetAttribute("name");
                        if (nameAttr == "notes") //пропуск сносок (они уже обработаны ранее)
                        { await reader.SkipAsync(); continue; }
                        // Это основной боди
                        bodyIdx = bodyCount;
                        inMainBody = true;
                        continue;
                    }

                    if (localName == "binary") //картинки тоже уже обработаны
                    { await reader.SkipAsync(); continue; }

                    if (!inMainBody) continue; // если иной элемент (типа секции) - но не в главном боди,
                    //то тоже нужно пропустить - не нужно либо уже обработано

                    //глава или раздел
                    if (localName == "section")
                    {
                        int depth = sectionStack.Count; // глубина вложенности: 0 = прямые дочери body

                        // Убедимся, что счётчик для этой глубины существует
                        while (sectionCounters.Count <= depth) sectionCounters.Add(0);

                        sectionCounters[depth]++;   // порядковый номер секции на данной глубине
                        int ordinal = sectionCounters[depth];

                        // Сохраняем текущий elemIdx родителя — восстановим при выходе
                        sectionStack.Push((ordinal, elemIdx));
                        elemIdx = 0; //нумерация внутри новой секции с нуля
                        continue;
                    }

                    if (localName == "a") //номер страницы (сейчас только номер)
                    {
                        var anchorId = reader.GetAttribute("id");
                        var pageNum = TryParsePageNumber(anchorId); //сам номер страницы
                        //если номер был указан корректно, то его нужно обработать
                        if (pageNum.HasValue)
                        {
                            elemIdx++; //это тоже элемент
                            var pe = new ParsedElement //новый элемент
                            {
                                Type = "pn",
                                Content = pageNum,
                                Text = null,
                                Xp = BuildXp(bodyIdx, sectionStack, elemIdx),
                                GlobalIndex = globalIdx
                            };
                            lastElement = pe;
                            currentPart.Add(pe);
                            globalIdx++;
                            totalElements++;

                            if (firstPartGlobal == null)
                            {
                                firstPartGlobal = pe.GlobalIndex;
                                firstPartXp = pe.Xp;
                            }
                            //если накоплено достаточно фрагментов, то нужно их сбросить
                            //в хранилище и очистить
                            if (currentPart.Count >= options.TargetPartSize)
                            {
                                partIndex = await FlushPartAsync(bookId, currentPart,
                                    tocParts, partIndex, ct);
                                currentPart.Clear();
                            }
                        }
                        //содержимое не нужно (даже если есть)
                        if (!reader.IsEmptyElement)
                            await reader.SkipAsync();
                        continue;
                    }

                    //разные возможные текстовые элементы  (или image)
                    if (localName is "p" or "title" or "subtitle" or "empty-line"
                        or "image")
                    {
                        elemIdx++;
                        var xp = BuildXp(bodyIdx, sectionStack, elemIdx);

                        ParsedElement? pe = null;

                        if (localName == "empty-line")
                        {
                            pe = new ParsedElement
                            {
                                Type = "br",
                                Content = null,
                                Text = null,
                                Xp = xp,
                                GlobalIndex = globalIdx
                            };
                            if (!reader.IsEmptyElement) await reader.SkipAsync();
                        }
                        //ссылка на картинку в тексте - сама картинка указывается ссылкой
                        //в href на тот тег binary, который расположен в конце файла
                        else if (localName == "image")
                        {
                            var href = reader.GetAttribute("href")?.TrimStart('#');
                            if (href != null && imageMap.TryGetValue(href, out var imgFile))
                            {
                                pe = new ParsedElement
                                {
                                    Type = "img",
                                    Content = new ImgSegment { Src = imgFile },
                                    Text = null,
                                    Xp = xp,
                                    GlobalIndex = globalIdx
                                };
                            }
                            if (!reader.IsEmptyElement) await reader.SkipAsync();
                        }
                        else
                        {
                            //все остальное - уже текстовое
                            var outerXml = await reader.ReadOuterXmlAsync(); //тег целиком
                            var (content, flatText) = ParseMixedXml(outerXml, notes, imageMap); //он могу быть смешанным
                            //(с другими сносками и картинками и т.д.) - в общем, рекурсивная структура гипотетически

                            if (content != null || localName != "p") //контента может не быть, 
                                                                     //но это может быть заголовок и т.д. - в таком случае, тоже сохранить (потом можно проверить
                                                                     //и дополнить, если что)
                            {
                                pe = new ParsedElement
                                {
                                    Type = localName,
                                    Content = content, //сегменты текста, ссылки и т.д.
                                    Text = flatText, //мог быть и текст просто
                                    Xp = xp,
                                    GlobalIndex = globalIdx
                                };
                            }
                        }

                        if (pe == null) continue; //непонятно какой тег - пропустить

                        //Заголовок верхнего уровня (это стек,
                        //если там до одного элемента, значит, заголовок верхнего уровня)
                        //Если потребуется составить оглавление многоуровневое - 
                        //подправить потом здесь
                        if (pe.Type == "title" && sectionStack.Count <= 1)
                            UpdateChapters(allChapters, pe, globalIdx);

                        fullLength += pe.Text?.Length ?? 0; //добавление длины текста
                        lastElement = pe;
                        currentPart.Add(pe);
                        globalIdx++;
                        totalElements++;

                        if (firstPartGlobal == null)
                        {
                            firstPartGlobal = pe.GlobalIndex;
                            firstPartXp = pe.Xp;
                        }

                        //Накоплено достаточно - сбросить и очистить
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
                    if (localName == "body")
                        inMainBody = false;

                    //если это была секция, то нужно восстановить счётчик элементов родительского уровня
                    if (localName == "section" && sectionStack.Count > 0)
                    {
                        var (_, parentElemCount) = sectionStack.Pop();
                        elemIdx = parentElemCount; // восстановить счётчик родителя
                    }
                }
            }

            //Если не сбросили содержимое послежнего фрагмента,
            //при этом это содержимое есть - то сброс принудительно
            if (currentPart.Count > 0)
                partIndex = await FlushPartAsync(bookId, currentPart, tocParts, partIndex, ct);

            //При этом нужно не забыть про главу
            if (allChapters.Count > 0 && lastElement != null)
            {
                //var last = allChapters[^1]; //индекс от конца - самый послежний элемент
                //allChapters[^1] = new TocChapter
                //{
                //    S = last.S,
                //    E = lastElement.GlobalIndex,
                //    T = last.T
                //};
                allChapters[^1].E = lastElement.GlobalIndex; //раньше был тип record, 
                //зачем мне он нужен вообще? Пусть класс будет!
            }

            var bookTitle = meta.Title ?? allChapters.FirstOrDefault(c => c.T != null)?.T //title
                ?? string.Empty; //заголовок книги - алгоритм мог найти его еще
            //в секции описания, либо просто взять самую первую секцию из боди

            var tocDoc = new TocDocument
            {
                Meta = meta,
                FullLength = fullLength, //длина текста целиком
                Body = totalElements > 0
                    ? [new TocBody
                    {
                        S = tocParts.FirstOrDefault()?.S ?? 0,
                        E = tocParts.LastOrDefault()?.E  ?? 0,
                        T = bookTitle,
                        C = allChapters
                    }]
                    : [], //в файле fb2 один боди, но потом можно либо сохранять просто фрагменты,
                //либо несколько боди - если будет иной формат, типа fb3 или epub (там их уже несколько)
                //кроме того, здесь сохраняются главы-чаптеры (ААА, нужно потом дополнить!!!)
                Parts = tocParts //список фрагментов нужен обязательно - для быстрого перехода по оглавлению к нужному фрагменту
            };

            return (totalElements, tocDoc);
        }

        // Парсит смешанный XML-фрагмент
        // в объект Content (string или List или object) и plain-text
        // Использует легковесный XmlReader — не создаёт полный DOM

        // Парсит смешанный XML-фрагмент
        // в объект Content (string или List или object) и plain-text
        // Использует легковесный XmlReader — не создаёт полный DOM
        private (object? content, string? flatText) ParseMixedXml(
            string outerXml,
            Dictionary<string, ParsedNote> notes,
            Dictionary<string, string> imageMap)
        {
            _logger.LogDebug("parse mixed xml");
            var mixed = new List<object>(); //это может быть большой список из разного содержимого, по идее
            var buf = new StringBuilder();

            void FlushBuf() //сбросить накопленную строку
            {
                var s = CollapseWhitespace(buf.ToString()); //удалить лишние пробелы
                if (s.Length > 0) mixed.Add(s);
                buf.Clear();
            }

            using var r = XmlReader.Create( //напрямую в констурктор, как оказалось, не принимает строку, поэтому использовал stringReader
                new StringReader(outerXml),
                new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }); //по умолчанию prohibit

            //Здесь корневой тег, его нужно пропустить
            r.Read();
            int rootDepth = r.Depth; //абсолютная глубина

            // Вспомогательный метод: читает содержимое текущего элемента как plain-текст,
            // рекурсивно обходя любые дочерние теги. Позиция читателя после вызова —
            // на EndElement обрабатываемого тега (или сразу после пустого элемента).
            // Используется для «понятных» инлайн-тегов (strong, emphasis), у которых
            // теоретически может быть вложенная разметка (например, <strong><em>...</em></strong>).
            static string ReadInnerText(XmlReader r)
            {
                if (r.IsEmptyElement) return string.Empty;
                var sb = new StringBuilder();
                int depth = r.Depth; // глубина открывающего тега
                while (r.Read() && r.Depth > depth)
                {
                    if (r.NodeType is XmlNodeType.Text or XmlNodeType.SignificantWhitespace)
                        sb.Append(r.Value);
                    // все вложенные теги прозрачно проходятся; их текстовые узлы будут
                    // подхвачены на следующих итерациях, пока глубина больше depth
                }
                // после цикла r стоит на EndElement родительского тега — это ожидаемое состояние
                return sb.ToString();
            }

            while (r.Read())
            {
                if (r.Depth == rootDepth) break; //на всякий случай выход сразу по окончании корневого тега
                _logger.LogDebug("type {Type}--->node with name {Name}, with value {Value}",
                        r.NodeType, r.LocalName, r.Value);
                switch (r.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.SignificantWhitespace: //например, в теге p с xml:space=preserve пробелы с переносами или иным содержимым могут стать именно этим
                        buf.Append(r.Value);
                        break;

                    case XmlNodeType.Element:
                        switch (r.LocalName)
                        {
                            case "strong":
                                {
                                    FlushBuf(); //сброс уже накопленного текста
                                    // ReadInnerText безопасно обходит любые вложенные теги,
                                    // поэтому <strong><em>текст</em></strong> тоже корректно обработается
                                    var inner = ReadInnerText(r).Trim();
                                    if (inner.Length > 0) mixed.Add(new StSegment { C = inner }); //сегмент жирного текста
                                }
                                break;

                            case "emphasis": //курсив - аналогично
                                {
                                    FlushBuf();
                                    var inner = ReadInnerText(r).Trim();
                                    if (inner.Length > 0) mixed.Add(new EmSegment { C = inner });
                                }
                                break;

                            case "a": //пока только якоря (для номеров страниц) или ссылки для сносок
                                {
                                    var noteType = r.GetAttribute("type");
                                    var href = r.GetAttribute("href")?.TrimStart('#');

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
                                        // ReadInnerText безопасен для смешанного содержимого ссылки
                                        var label = ReadInnerText(r);

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
                                    var href = r.GetAttribute("href")?.TrimStart('#');
                                    if (href != null && imageMap.TryGetValue(href, out var imgFile))
                                    {
                                        FlushBuf();
                                        mixed.Add(new ImgSegment { Src = imgFile });
                                    }
                                    if (!r.IsEmptyElement) r.Skip();
                                    break;
                                }

                            default:
                                // Тег-обёртка (например, <p> внутри <title>, или любой иной неизвестный тег).
                                // Нельзя вызывать ReadElementContentAsString(), если внутри есть дочерние теги
                                // (это приведёт к InvalidOperationException).
                                // Вместо этого используем ReadInnerText(), который безопасно рекурсивно
                                // извлекает весь текст, а форматирующие сегменты (strong/emphasis/a/image)
                                // внутри таких обёрток будут потеряны как сегменты, но их текст сохранится.
                                // Альтернатива — рекурсивный вызов ParseMixedXml с outerXml дочернего тега,
                                // но тогда нужно вычитывать outerXml через r.ReadOuterXml(), что меняет
                                // позицию читателя и усложняет логику.
                                buf.Append(ReadInnerText(r));
                                break;
                        }
                        break;
                }
            }

            FlushBuf();

            if (mixed.Count == 0)
                return (null, null);

            //если все строки, то можно склеить в одну
            if (mixed.All(x => x is string))
            {
                var plain = string.Concat(mixed.Cast<string>()).Trim(); //приведение типа к строке
                return plain.Length > 0 ? (plain, plain) : (null, null);
            }

            var flatText = string.Concat(mixed.OfType<string>()).Trim();
            var s = string.IsNullOrEmpty(flatText) ? null : flatText; //если все строки были только пробелами
            return (mixed, s);
        }





        //private (object? content, string? flatText) ParseMixedXml(
        //    string outerXml,
        //    Dictionary<string, ParsedNote> notes,
        //    Dictionary<string, string> imageMap)
        //{
        //    _logger.LogDebug("parse mixed xml");
        //    var mixed = new List<object>(); //это может быть большой список из разного содержимого, по идее
        //    var buf = new StringBuilder();

        //    void FlushBuf() //сбросить накопленную строку
        //    {
        //        var s = CollapseWhitespace(buf.ToString()); //удалить лишние пробелы
        //        if (s.Length > 0) mixed.Add(s);
        //        buf.Clear();
        //    }

        //    using var r = XmlReader.Create( //напрямую в констурктор, как оказалось, не принимает строку, поэтому использовал stringReader
        //        new StringReader(outerXml),
        //        new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }); //по умолчанию prohibit

        //    //Здесь корневой тег, его нужно пропустить
        //    r.Read();
        //    int rootDepth = r.Depth; //абсолютная глубина

        //    while (r.Read())
        //    {
        //        if (r.Depth == rootDepth) break; //на всякий случай выход сразу по окончании корневого тега
        //        _logger.LogDebug("type {Type}--->node with name {Name}, with value {Value}",
        //                r.NodeType, r.LocalName, r.Value);
        //        switch (r.NodeType)
        //        {
        //            case XmlNodeType.Text:
        //            case XmlNodeType.SignificantWhitespace: //например, в теге p с xml:space=preserve пробелы с переносами или иным содержимым могут стать именно этим
        //                buf.Append(r.Value);
        //                break;

        //            case XmlNodeType.Element:
        //                switch (r.LocalName)
        //                {
        //                    case "strong":
        //                        {

        //                            FlushBuf(); //сброс уже накопленного текста
        //                            var inner = r.ReadElementContentAsString().Trim();
        //                            if (inner.Length > 0) mixed.Add(new StSegment { C = inner }); //сегмент жирного текста
        //                        }
        //                        break;

        //                    case "emphasis": //курсив - аналогично
        //                        {
        //                            FlushBuf();
        //                            var inner = r.ReadElementContentAsString().Trim();
        //                            if (inner.Length > 0) mixed.Add(new EmSegment { C = inner });
        //                        }
        //                        break;

        //                    case "a": //пока только якоря (для номеров страниц) или ссылки для сносок
        //                        {
        //                            var noteType = r.GetAttribute("type");
        //                            var href = r.GetAttribute("href")?.TrimStart('#');

        //                            var anchorId = r.GetAttribute("id");
        //                            var pageNumber = TryParsePageNumber(anchorId);
        //                            if (pageNumber.HasValue)
        //                            {
        //                                FlushBuf();
        //                                mixed.Add(new PageNumberSegment { Pn = pageNumber.Value });
        //                                if (!r.IsEmptyElement) r.Skip();
        //                            }

        //                            else
        //                            {
        //                                var label = r.ReadElementContentAsString();

        //                                if (noteType == "note" && href != null
        //                                    && notes.TryGetValue(href, out var note))
        //                                {
        //                                    FlushBuf();
        //                                    mixed.Add(new NoteSegment
        //                                    {
        //                                        C = label,
        //                                        Xp = note.Xp,
        //                                        F = new FootnoteContent
        //                                        {
        //                                            Xp = note.Xp,
        //                                            C = note.Paragraphs
        //                                        }
        //                                    });
        //                                }
        //                                else
        //                                {
        //                                    buf.Append(label);
        //                                }

        //                            }
        //                            break;
        //                        }

        //                    case "image":
        //                        {
        //                            var href = r.GetAttribute("href")?.TrimStart('#');
        //                            if (href != null && imageMap.TryGetValue(href, out var imgFile))
        //                            {
        //                                FlushBuf();
        //                                mixed.Add(new ImgSegment { Src = imgFile });
        //                            }
        //                            if (!r.IsEmptyElement) r.Skip();
        //                            break;
        //                        }

        //                    default:
        //                        //прочие теги - только текст
        //                        buf.Append(r.ReadElementContentAsString());
        //                        break;
        //                }
        //                break;
        //        }
        //    }

        //    FlushBuf();

        //    if (mixed.Count == 0)
        //        return (null, null);

        //    //если все строки, то можно склеить в одну
        //    if (mixed.All(x => x is string))
        //    {
        //        var plain = string.Concat(mixed.Cast<string>()).Trim(); //приведение типа к строке
        //        return plain.Length > 0 ? (plain, plain) : (null, null);
        //    }

        //    var flatText = string.Concat(mixed.OfType<string>()).Trim();
        //    var s = string.IsNullOrEmpty(flatText) ? null : flatText; //если все строки были только пробелами
        //    return (mixed, s);
        //}

        // Сериализует накопленный фрагмент, сохраняет в хранилище,
        // добавляет запись в список tocParts
        private async Task<int> FlushPartAsync(
            string bookId,
            List<ParsedElement> part,
            List<TocPart> tocParts,
            int partIndex,
            CancellationToken ct)
        {
            if (part.Count == 0) return partIndex; //проверка на пустой фрагмент - изменений нет

            var first = part[0];
            var last = part[^1];
            var fileName = $"{partIndex:D3}.json"; //000.json,..., строка формата, целое число, 3 цифры

            var items = part.Select(MapToPartElement).ToList(); //преобразовать ParsedElement в PartElement для сохранения, группа методов или делегат (стрелочная функция)
            var json = JsonSerializer.Serialize(items, JsonOpts);
            var bytes = Encoding.UTF8.GetByteCount(json);

            await _storage.SaveChunkAsync(bookId, fileName, json, false, ct);

            tocParts.Add(new TocPart
            {
                S = first.GlobalIndex,
                E = last.GlobalIndex,
                Xps = first.Xp,
                Xpe = last.Xp,
                Url = fileName
            });

            partIndex++;
            return partIndex;
        }

        //обновляет список глав
        private static void UpdateChapters(
            List<TocChapter> chapters,
            ParsedElement titleElement,
            int globalIdx)
        {
            //закрытие предыдущей главы
            if (chapters.Count > 0)
            {
                //var prev = chapters[^1];
                //chapters[^1] = new TocChapter
                //{
                //    S = prev.S,
                //    E = globalIdx - 1,
                //    T = prev.T
                //};
                chapters[^1].E = globalIdx - 1;
            }
            //открытие новой
            chapters.Add(new TocChapter
            {
                S = globalIdx,
                E = globalIdx,
                T = titleElement.Text
            });
        }

        // Метаданные (название книги) (парсинг outerXml блока description)

        private static BookMeta ParseMetaFromXml(string descXml)
        {
            using var r = XmlReader.Create(new StringReader(descXml),
                new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });


            string? title = null;
            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.LocalName)
                    {
                        case "book-title": title = r.ReadElementContentAsString().Trim(); break;
                    }
                }
            }
            return new BookMeta
            {
                Title = title,
            };
        }

        //парсинг номера страницы - обязательно как pN
        private static int? TryParsePageNumber(string? id)
        {
            if (id is null || id.Length < 2 || id[0] != 'p') return null;
            //взять строку без первого символа без аллокации
            return int.TryParse(id.AsSpan(1), out var n) && n > 0 ? n : null;

        }

        private static XmlReader CreateXmlReader(Stream stream)
            => XmlReader.Create(stream, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                IgnoreWhitespace = false, //сохранение пробельных узлов
                Async = true //поддержка асинк методов
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
                return CollapseWhitespace(sb.ToString()); //убрать лишние пробелы
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// Строит массив координат (xp) для текущей позиции в дереве документа.
        /// xp[0] = bodyIdx
        /// xp[1..^1] = порядковые номера секций на каждом уровне вложенности (от внешних к внутренним)
        /// xp[^1] = elemIdx — номер текущего элемента внутри самой глубокой секции
        /// Пример: body/section[2]/section[1]/p[3]  →  [bodyIdx, 2, 1, 3]
        /// </summary>
        private static int[] BuildXp(
            int bodyIdx,
            Stack<(int SectionOrdinal, int ParentElemCount)> sectionStack,
            int elemIdx)
        {
            // Стек хранится «верхушка = самый вложенный уровень»,
            // поэтому для построения пути от корня к листу нужно перевернуть порядок.
            var frames = sectionStack.ToArray(); // [innermost, ..., outermost]
            // Длина: 1 (body) + глубина секций + 1 (elem)
            var xp = new int[1 + frames.Length + 1];
            xp[0] = bodyIdx;
            for (int i = 0; i < frames.Length; i++)
                xp[i + 1] = frames[frames.Length - 1 - i].SectionOrdinal; // от внешнего к внутреннему
            xp[^1] = elemIdx;
            return xp;
        }

        //преобразование предварительной модели в конечную
        private static PartElement MapToPartElement(ParsedElement pe)
        {
            object? c = pe.Content;

            // pn-элемент: Content — int (номер страницы)
            if (pe.Type == "pn" && c is int pageNum)
            {
                return new PartElement
                {
                    T = pe.Type,
                    Xp = pe.Xp,
                    C = pageNum
                };
            }

            // Если Content — одиночный ImgSegment, передаём как список из одного
            if (c is ImgSegment img)
                c = new List<object> { img };

            return new PartElement
            {
                T = pe.Type,
                Xp = pe.Xp,
                C = c
            };
        }

        private static string ContentTypeToExtension(string contentType) =>
            contentType.ToLowerInvariant() switch
            {
                "image/png" => ".png",
                _ => ".jpg"
            };

        private static readonly Regex WhitespaceRegex = new Regex(@"\s+"); //регулярное выражение
        //верабльный литерал (игнорирование обратных слешей, чтобы не было \\)
        //пробел, переносы и табуляции

        private static string CollapseWhitespace(string s)
            => WhitespaceRegex.Replace(s, " ").Trim();
    }
}