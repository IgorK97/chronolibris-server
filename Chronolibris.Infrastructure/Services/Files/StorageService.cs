using System.Text;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Chronolibris.Infrastructure.Services.Files
{
    public sealed class StorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly BookStorageOptions _bookOpts;
        private readonly ILogger<StorageService> _logger;

        public StorageService(
            IMinioClient minioClient,
            IOptions<BookStorageOptions> bookOpts,
            ILogger<StorageService> logger)
        {
            _minioClient = minioClient;
            _bookOpts = bookOpts.Value;
            _logger = logger;
        }

        public async Task<string> SaveBookSourceAsync(string bookId, string extension, Stream data, CancellationToken ct = default)
        {
            var key = $"{bookId}/source{extension}";
            await UploadAsync(_bookOpts.BooksBucket, key, data, ResolveContentType(extension), ct);
            return key;
        }

        public Task<Stream?> ReadBookSourceAsync(string bookId, string extension, CancellationToken ct = default)
        {
            return ReadAsync(_bookOpts.BooksBucket, $"{bookId}/source{extension}", ct);
        }

        public async Task SaveCoverAsync(string bookId, string fileName, Stream data, string contentType, CancellationToken ct = default)
        {
            await UploadAsync(_bookOpts.CoversBucket, $"{bookId}/{fileName}", data, contentType, ct);
        }

        public async Task SaveImageAsync(string bookId, string fileName, Stream data, string contentType, CancellationToken ct = default)
        {
            await UploadAsync(_bookOpts.PublicImagesBucket, $"{bookId}/{fileName}", data, contentType, ct);
        }

        public async Task SaveChunkAsync(string bookId, string fileName, string content, bool isToc = false, CancellationToken ct = default)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using var ms = new MemoryStream(bytes);
            var key = isToc ? $"{bookId}/toc.json" : $"{bookId}/chunks/{fileName}";
            await UploadAsync(_bookOpts.BooksBucket, key, ms, "application/json", ct);
        }

        public async Task<string?> ReadChunkAsync(string bookId, string fileName, bool isToc, CancellationToken ct = default)
        {
            var key = isToc ? $"{bookId}/toc.json" : $"{bookId}/chunks/{fileName}";
            var stream = await ReadAsync(_bookOpts.BooksBucket, key, ct);
            if (stream is null) return null;

            using var reader = new StreamReader(stream, Encoding.UTF8);
            return await reader.ReadToEndAsync(ct);
        }

        public async Task<bool> ChunkExistsAsync(string bookId, string fileName, bool isToc, CancellationToken ct = default)
        {
            var key = isToc ? $"{bookId}/toc.json" : $"{bookId}/chunks/{fileName}";
            try
            {
                await _minioClient.StatObjectAsync(new StatObjectArgs().WithBucket(_bookOpts.BooksBucket).WithObject(key), ct);
                return true;
            }
            catch (ObjectNotFoundException)
            {
                return false;
            }
        }

        public async Task DeleteBookDataAsync(string bookId, CancellationToken ct = default)
        {
            await RecursiveDeleteAsync(_bookOpts.BooksBucket, $"{bookId}/", ct);
            await RecursiveDeleteAsync(_bookOpts.PublicImagesBucket, $"{bookId}/", ct);
            //await RecursiveDeleteAsync(_bookOpts.CoversBucket, $"{bookId}/", ct);
        }

        public async Task DeleteFileAsync(string bucket, string objectKey, CancellationToken ct = default)
        {
            try
            {
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucket).WithObject(objectKey), ct);

            }
            catch (ObjectNotFoundException) { }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении в MinIO. Bucket: {Bucket}, Key: {Key}",
                    bucket, objectKey);
                throw;
            }
        }

        private async Task UploadAsync(string bucket, string key, Stream data, string contentType, CancellationToken ct = default)
        {
            try
            {
                await EnsureBucketAsync(bucket, ct);
                await _minioClient.PutObjectAsync(new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithStreamData(data)
                    .WithObjectSize(data.Length)
                    .WithContentType(contentType), ct);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Ошибка записи в MinIO. Bucket: {Bucket}, Key: {Key}", bucket, key);
                throw;
            }
        }

        private async Task<Stream?> ReadAsync(string bucket, string key, CancellationToken ct = default)
        {
            try
            {
                var ms = new MemoryStream();
                await _minioClient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithCallbackStream(async (s, t) => await s.CopyToAsync(ms, t)), ct);
                ms.Position = 0;
                return ms;
            }
            catch (ObjectNotFoundException) { 
                return null; 
            }
        }

        private async Task RecursiveDeleteAsync(string bucket, string prefix, CancellationToken ct = default)
        {
            var items = _minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
                .WithBucket(bucket)
                .WithPrefix(prefix)
                .WithRecursive(true), ct);
            await foreach(var item in items)
            {
                await DeleteFileAsync(bucket, item.Key, ct);
            }
        }

        private async Task EnsureBucketAsync(string bucket, CancellationToken ct)
        {
            var found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), ct);
            if (!found) await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), ct);
        }

        private static string ResolveContentType(string ext) => ext.ToLowerInvariant() switch
        {
            ".fb2" => "application/xml", //на что это влияет?
            _ => "application/octet-stream"
        };
    }
}