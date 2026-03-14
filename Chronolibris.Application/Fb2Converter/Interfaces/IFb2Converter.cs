using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Domain.Models;

namespace Chronolibris.Application.Fb2Converter.Interfaces
{
    /// <summary>
    /// Конвертирует FB2-файл в набор JSON-фрагментов и сохраняет их через <see cref="IBookStorage"/>.
    /// </summary>
    public interface IFb2Converter
    {
        /// <summary>
        /// Конвертирует поток с FB2-данными в фрагменты, сохраняет их в хранилище и
        /// возвращает метаданные всех созданных файлов (для записи в БД).
        /// </summary>
        /// <param name="fb2Stream">Входной поток с содержимым FB2-файла.</param>
        /// <param name="bookId">
        /// Уникальный идентификатор книги. Если null — берётся из &lt;id&gt; блока
        /// document-info; если там тоже нет — генерируется новый GUID.
        /// </param>
        /// <param name="options">Параметры конвертации (размер фрагмента и др.).</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Результат конвертации с метаданными всех созданных объектов.</returns>
        Task<ConversionResult> ConvertAsync(
            Stream fb2Stream,
            long? bookId = null,
            ConversionOptions? options = null,
            CancellationToken cancellationToken = default);
    }
}
