using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Fb2Converter.Interfaces
{
    /// <summary>
    /// Абстракция хранилища для сохранения фрагментов книги (MinIO / любой S3-совместимый бэкенд).
    /// </summary>
    public interface IBookStorage
    {
        /// <summary>
        /// Сохраняет JSON-фрагмент.
        /// </summary>
        /// <param name="bookId">Уникальный идентификатор книги (UUID).</param>
        /// <param name="fileName">Имя файла, например «000.js», «toc.json».</param>
        /// <param name="content">Содержимое в виде строки.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task SaveAsync(string bookId, string fileName, string content,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Читает ранее сохранённый фрагмент. Возвращает null, если файл не найден.
        /// </summary>
        Task<string?> ReadAsync(string bookId, string fileName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Проверяет существование объекта в хранилище.
        /// </summary>
        Task<bool> ExistsAsync(string bookId, string fileName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Сохраняет бинарный файл (изображение) в хранилище.
        /// </summary>
        /// <param name="bookId">UUID книги.</param>
        /// <param name="fileName">Имя файла, например «1.jpg».</param>
        /// <param name="data">Байты изображения.</param>
        /// <param name="contentType">MIME-тип, например «image/jpeg».</param>
        Task SaveImageAsync(string bookId, string fileName, byte[] data, string contentType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Читает произвольный объект как поток байт.
        /// Используется для получения исходного FB2 перед конвертацией.
        /// Возвращает null если объект не найден.
        /// </summary>
        Task<Stream?> ReadStreamAsync(string objectName,
            CancellationToken cancellationToken = default);
    }
}
