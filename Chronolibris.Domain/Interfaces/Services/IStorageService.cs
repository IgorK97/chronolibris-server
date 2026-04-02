using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Interfaces.Services
{

    public interface IStorageService
    {
        string PublicCoversBucket { get; }

        Task DeleteAsync(string bucketName, string objectKey, CancellationToken ct = default);

        /// <summary>
        /// Сохраняет исходный файл книги и возвращает его ключ (storageUrl) для сохранения в БД.
        /// ContentType определяется автоматически по расширению.
        /// </summary>
        /// <param name="bookId">Идентификатор книги в БД.</param>
        /// <param name="extension">Расширение файла с точкой, напр. <c>.fb2</c>.</param>
        /// <param name="data">Поток с содержимым файла.</param>
        /// <returns>Ключ объекта (storageUrl), напр. <c>v1/{bookId}/source.fb2</c>.</returns>
        Task<string> SaveBookSourceAsync(
            string bookId,
            string extension,
            Stream data,
            CancellationToken ct = default);

        /// <summary>
        /// Открывает исходный файл книги для чтения.
        /// Возвращает <c>null</c>, если файл не найден.
        /// </summary>
        Task<Stream?> ReadBookSourceAsync(
            string bookId,
            string extension,
            CancellationToken ct = default);

        /// <summary>
        /// Читает произвольный объект из бакета книг по полному ключу,
        /// хранящемуся в БД (storageUrl).
        /// Используется когда bookId и расширение неизвестны — только ключ.
        /// Возвращает <c>null</c>, если объект не найден.
        /// </summary>
        Task<Stream?> ReadByStorageUrlAsync(
            string storageUrl,
            CancellationToken ct = default);


        /// <summary>
        /// Сохраняет JSON-фрагмент книги.
        /// Ключ объекта: <c>v1/{bookId}/chunks/{fileName}</c>
        /// </summary>
        Task SaveChunkAsync(
            string bookId,
            string fileName,
            string content,
            string type,
            CancellationToken ct = default);

        /// <summary>
        /// Читает JSON-фрагмент книги.
        /// Возвращает <c>null</c>, если фрагмент не найден.
        /// </summary>
        Task<string?> ReadChunkAsync(
            string bookId,
            string fileName,
            string type,
            CancellationToken ct = default);

        /// <summary>
        /// Проверяет, существует ли фрагмент книги.
        /// </summary>
        Task<bool> ChunkExistsAsync(
            string bookId,
            string fileName,
            string type,
            CancellationToken ct = default);


        /// <summary>
        /// Сохраняет изображение, извлечённое из книги.
        /// Ключ объекта: <c>v1/{bookId}/images/{fileName}</c>
        /// </summary>
        Task SavePublicBookImageAsync(
            string bookId,
            string fileName,
            byte[] data,
            string contentType,
            CancellationToken ct = default);

        Task SaveCoverAsync(string bookId, string fileName, byte[] data, string contentType, CancellationToken ct = default);

        /// <summary>
        /// Загружает произвольный файл (обложку и т.п.) и возвращает его ключ
        /// (<c>storageUrl</c>), который следует сохранить в БД.
        /// Ключ объекта: <c>uploads/{guid}_{fileName}</c>
        /// </summary>
        /// <returns>Ключ объекта внутри бакета (storageUrl).</returns>
        Task<string> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            CancellationToken ct = default);

        /// <summary>
        /// Удаляет ранее загруженный файл по ключу, возвращённому методом
        /// <see cref="UploadFileAsync"/>.
        /// Не бросает исключение, если файл уже удалён.
        /// </summary>
        /// <param name="storageUrl">Ключ объекта внутри бакета (то, что хранится в БД).</param>
        Task DeleteFileAsync(
            string storageUrl,
            CancellationToken ct = default);
    }
}
