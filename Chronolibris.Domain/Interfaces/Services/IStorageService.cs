namespace Chronolibris.Domain.Interfaces.Services
{

    public interface IStorageService
    {
        //Файлы книг (сохранение, чтение)
        Task<string> SaveBookSourceAsync(string bookId, string extension, Stream data, CancellationToken ct = default);

        Task<Stream?> ReadBookSourceAsync(string bookId, string extension, CancellationToken ct = default);

        //Обложки книг и картинки
        Task SaveCoverAsync(string bookId, string fileName, Stream data, string contentType, CancellationToken ct = default);
        Task SaveImageAsync(string bookId, string fileName, Stream data, string contentType, CancellationToken ct = default);
        Task<Stream?> ReadImageAsync(string bookId, string fileName, CancellationToken ct = default);

        //Фрагменты книг (сохранение, чтение, проверка на существование)
        Task SaveChunkAsync(string bookId, string fileName, string content, bool isToc = false, CancellationToken ct = default);

        Task<string?> ReadChunkAsync(string bookId, string fileName, bool isToc = false, CancellationToken ct = default);

        Task<bool> ChunkExistsAsync(string bookId, string fileName, bool isToc = false, CancellationToken ct = default);

        //Общие операции (удаление)
        Task DeleteFileAsync(string bucket, string objectKey, CancellationToken ct = default);
        Task DeleteBookDataAsync(string bookId, CancellationToken ct = default);   
    }
}