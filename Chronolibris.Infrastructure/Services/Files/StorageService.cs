using System.Text;
using Chronolibris.Domain.Interfaces.Services;
using Chronolibris.Infrastructure.Utils;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Chronolibris.Infrastructure.Services.Files
{
    public sealed class StorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly BookStorageOptions _bookOpts;
        private readonly UploadStorageOptions _uploadOpts;
        public string PublicCoversBucket => _bookOpts.CoversBucket;

        public StorageService(
            IMinioClient minioClient,
            IOptions<BookStorageOptions> bookOpts,
            IOptions<UploadStorageOptions> uploadOpts)
        {
            _minioClient = minioClient;
            _bookOpts = bookOpts.Value;
            _uploadOpts = uploadOpts.Value;
        }


        public async Task<string> SaveBookSourceAsync(
            string bookId, string extension, Stream data, CancellationToken ct = default)
        {
            var key = BookSourceKey(bookId, extension);
            await EnsureBucketAsync(_bookOpts.BooksBucket, ct);
            await PutAsync(_bookOpts.BooksBucket, key, data, data.Length, ResolveContentType(extension), ct);
            return key;
        }

        public async Task<Stream?> ReadBookSourceAsync(
            string bookId, string extension, CancellationToken ct = default)
            => await ReadAsync(_bookOpts.BooksBucket, BookSourceKey(bookId, extension), ct);

        public async Task<Stream?> ReadByStorageUrlAsync(
            string storageUrl, CancellationToken ct = default)
            => await ReadAsync(_bookOpts.BooksBucket, storageUrl, ct);


        public async Task SaveChunkAsync(
            string bookId, string fileName, string content, string type,
            CancellationToken ct = default)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using var ms = new MemoryStream(bytes);
            await EnsureBucketAsync(_bookOpts.BooksBucket, ct);
            await PutAsync(_bookOpts.BooksBucket, ChunkKey(bookId, fileName, type),
                ms, bytes.Length, "application/json; charset=utf-8", ct);
        }

        public async Task<string?> ReadChunkAsync(
            string bookId, string fileName, string type, CancellationToken ct = default)
        {
            var stream = await ReadAsync(_bookOpts.BooksBucket, ChunkKey(bookId, fileName, type), ct);
            if (stream is null) return null;
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return await reader.ReadToEndAsync(ct);
        }

        public async Task<bool> ChunkExistsAsync(
            string bookId, string fileName, string type, CancellationToken ct = default)
        {
            try
            {
                await _minioClient.StatObjectAsync(
                    new StatObjectArgs()
                        .WithBucket(_bookOpts.BooksBucket)
                        .WithObject(ChunkKey(bookId, fileName, type)), ct);
                return true;
            }
            catch (ObjectNotFoundException) { return false; }
        }

        public async Task SavePublicBookImageAsync(
            string bookId, string fileName, byte[] data, string contentType,
            CancellationToken ct = default)
        {
            using var ms = new MemoryStream(data);
            await EnsureBucketAsync(_bookOpts.PublicImagesBucket, ct);
            await PutAsync(_bookOpts.PublicImagesBucket, CoverKey(bookId, fileName),
                ms, data.Length, contentType, ct);
        }

        public async Task SaveCoverAsync(string bookId, string fileName, byte[] data, string contentType, CancellationToken ct = default)
        {
            using var ms = new MemoryStream(data);
            await EnsureBucketAsync(_bookOpts.CoversBucket, ct);
            await PutAsync(_bookOpts.CoversBucket, CoverKey(bookId, fileName),
                ms, data.Length, contentType, ct);
        }


        public async Task<string> UploadFileAsync(
            Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
        {
            var key = UploadKey(fileName);
            await EnsureBucketAsync(_uploadOpts.UploadsBucket, ct);
            await PutAsync(_uploadOpts.UploadsBucket, key, fileStream, fileStream.Length, contentType, ct);
            return key;
        }

        public async Task DeleteFileAsync(string storageUrl, CancellationToken ct = default)
            => await DeleteAsync(_uploadOpts.UploadsBucket, storageUrl, ct);


        public async Task DeleteBookAsync(
            string bookId, string extension, CancellationToken ct = default)
        {
            await RecursiveDeleteAsync(_bookOpts.BooksBucket, $"{bookId}/", ct);
            await RecursiveDeleteAsync(_bookOpts.PublicImagesBucket, $"covers/{bookId}/", ct);
        }
        public async Task DeleteAsync(string bucketName, string objectKey, CancellationToken ct = default)
        {
            try
            {
                await _minioClient.RemoveObjectAsync(
                    new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectKey), ct);
            }
            catch (ObjectNotFoundException) { }
            catch (InvalidObjectNameException) { }
        }


        private async Task PutAsync(
            string bucket, string key, Stream data, long size, string contentType,
            CancellationToken ct)
        {
            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithStreamData(data)
                    .WithObjectSize(size)
                    .WithContentType(contentType), ct);
        }

        private async Task<MemoryStream?> ReadAsync(string bucket, string key, CancellationToken ct)
        {
            var ms = new MemoryStream();
            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithCallbackStream(async (stream, innerCt) =>
                        await stream.CopyToAsync(ms, innerCt)), ct);
            ms.Position = 0;
            return ms;
        }

        private async Task RecursiveDeleteAsync(string bucket, string prefix, CancellationToken ct)
        {
            var keys = new List<string>();
            await foreach (var item in _minioClient.ListObjectsEnumAsync(
                new ListObjectsArgs().WithBucket(bucket).WithPrefix(prefix).WithRecursive(true), ct))
                keys.Add(item.Key);

            foreach (var key in keys)
                await DeleteAsync(bucket, key, ct);
        }

        private async Task EnsureBucketAsync(string bucketName, CancellationToken ct)
        {
            if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), ct))
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), ct);
        }



        private static string BookSourceKey(string bookId, string ext) => $"{bookId}/source{ext}";
        private static string TocKey(string bookId) => $"{bookId}/toc.json";
        private static string ChunkKey(string bookId, string file, string type)
            => type == "toc" ? TocKey(bookId) : $"{bookId}/chunks/{file}";
        private static string CoverKey(string bookId, string fileName) => $"{bookId}/{fileName}";

        private static string UploadKey(string fileName) => $"uploads/{Guid.NewGuid()}_{fileName}";

        private static string ResolveContentType(string ext) => ext.ToLowerInvariant() switch
        {
            ".fb2" => "application/xml",
            ".epub" => "application/epub+zip",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}