//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.AccessControl;
//using System.Text;
//using System.Threading.Tasks;
//using Chronolibris.Domain.Interfaces.Services;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Microsoft.EntityFrameworkCore.Storage;
//using Microsoft.Extensions.Options;
//using Minio;
//using Minio.DataModel;
//using Minio.DataModel.Args;
//using Minio.Exceptions;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace Chronolibris.Infrastructure.DataAccess.Files
//{

//    public sealed class StorageService : IStorageService
//    {
//        //private readonly IMinioService _minio;
//        private readonly IMinioClient _minioClient;
//        private readonly BookStorageOptions _bookOpts;
//        private readonly UploadStorageOptions _uploadOpts;

//        public StorageService(
//            //IMinioService minio,
//            IMinioClient minioClient,
//            IOptions<BookStorageOptions> bookOpts,
//            IOptions<UploadStorageOptions> uploadOpts)
//        {
//            //_minio = minio;
//            _bookOpts = bookOpts.Value;
//            _uploadOpts = uploadOpts.Value;
//            _minioClient = minioClient;
//        }

//        public async Task<string> SaveBookSourceAsync(string bookId, string extension, Stream data,
//                                                      CancellationToken ct = default)
//        {
//            var key = BookSourceKey(bookId, extension);
//            var contentType = ResolveContentType(extension);

//            await EnsureBucketAsync(_bookOpts.BooksBucket, ct);

//            var args = new PutObjectArgs()
//                .WithBucket(_bookOpts.BooksBucket)
//                .WithObject(key)
//                .WithStreamData(data)
//                .WithObjectSize(data.Length)
//                .WithContentType(contentType);

//            await _minioClient.PutObjectAsync(args, ct);

//            //await _minio.PutAsync(_bookOpts.BooksBucket, key, data, data.Length, contentType, ct);
//            //return $"books/{bookId}";
//            return key;
//        }

//        private static string ResolveContentType(string extension) =>
//            extension.ToLowerInvariant() switch
//            {
//                ".fb2" => "application/xml",
//                ".epub" => "application/epub+zip",
//                ".pdf" => "application/pdf",
//                _ => "application/octet-stream"
//            };


//        /// <inheritdoc/>
//        public async Task<Stream?> ReadBookSourceAsync(
//            string bookId, string extension,
//            CancellationToken ct = default)
//        {
//            var key = BookSourceKey(bookId, extension);

//            var ms = new MemoryStream();

//            var args = new GetObjectArgs()
//                .WithBucket(_bookOpts.BooksBucket)
//                .WithObject(key)
//                .WithCallbackStream(async (stream, innerCt) =>
//                {
//                    await stream.CopyToAsync(ms, innerCt);
//                });

//            await _minioClient.GetObjectAsync(args, ct);

//            ms.Position = 0;
//            return ms;

//            //return _minio.GetAsync(_bookOpts.BooksBucket, key, ct);
//        }

//        public async Task<Stream?> ReadByStorageUrlAsync(string storageUrl, CancellationToken ct = default)
//        {
//            var ms = new MemoryStream();
//            var args = new GetObjectArgs()
//              .WithBucket(_bookOpts.BooksBucket)
//              .WithObject(storageUrl)
//              .WithCallbackStream(async (stream, innerCt) =>
//              {
//                  await stream.CopyToAsync(ms, innerCt);
//              });

//            await _minioClient.GetObjectAsync(args, ct);
//            ms.Position = 0;
//            return ms;
//        }


//        /// <inheritdoc/>
//        public async Task SaveChunkAsync(
//            string bookId, string fileName, string content, string type,
//            CancellationToken ct = default)
//        {
//            var bytes = Encoding.UTF8.GetBytes(content);
//            using var ms = new MemoryStream(bytes);
//            var key = ChunkKey(bookId, fileName, type);

//            await EnsureBucketAsync(_bookOpts.BooksBucket, ct);

//            var args = new PutObjectArgs()
//                .WithBucket(_bookOpts.BooksBucket)
//                .WithObject(key)
//                .WithStreamData(ms)
//                .WithObjectSize(bytes.Length)
//                .WithContentType("application/json; charset=utf-8");

//            await _minioClient.PutObjectAsync(args, ct);

//            //await _minioClient.PutAsync(
//            //    _bookOpts.BooksBucket, key, ms, bytes.Length,
//            //    "application/json; charset=utf-8", ct);
//        }

//        /// <inheritdoc/>
//        public async Task<string?> ReadChunkAsync(
//            string bookId, string fileName,string type,
//            CancellationToken ct = default)
//        {
//            var key = ChunkKey(bookId, fileName, type);

//            var ms = new MemoryStream();

//            var args = new GetObjectArgs()
//                .WithBucket(_bookOpts.BooksBucket)
//                .WithObject(key)
//                .WithCallbackStream(async (stream, innerCt) =>
//                {
//                    await stream.CopyToAsync(ms, innerCt);
//                });

//            await _minioClient.GetObjectAsync(args, ct);

//            ms.Position = 0;
//            //return ms;

//            //await using var stream = await _minio.GetAsync(_bookOpts.BooksBucket, key, ct);
//            //if (stream is null) return null;

//            using var reader = new StreamReader(ms, Encoding.UTF8);
//            return await reader.ReadToEndAsync(ct);
//        }

//        /// <inheritdoc/>
//        public async Task<bool> ChunkExistsAsync(
//            string bookId, string fileName,string type,
//            CancellationToken ct = default)
//        {
//            var key = ChunkKey(bookId, fileName, type);

//            try
//            {
//                var args = new StatObjectArgs()
//                    .WithBucket(_bookOpts.BooksBucket)
//                    .WithObject(key);

//                await _minioClient.StatObjectAsync(args, ct);
//                return true;
//            }
//            catch (ObjectNotFoundException)
//            {
//                return false;
//            }
//            //return _minioClient.ExistsAsync(_bookOpts.BooksBucket, key, ct);
//        }


//        /// <inheritdoc/>
//        public async Task SavePublicBookImageAsync(
//            string bookId, string fileName, byte[] data, string contentType,
//            CancellationToken ct = default)
//        {
//            using var ms = new MemoryStream(data);
//            var key = BookImageKey(bookId, fileName);

//            await EnsureBucketAsync(_bookOpts.PublicImagesBucket, ct);

//            var args = new PutObjectArgs()
//                .WithBucket(_bookOpts.PublicImagesBucket)
//                .WithObject(key)
//                .WithStreamData(ms)
//                .WithObjectSize(data.Length)
//                .WithContentType(contentType);

//            await _minioClient.PutObjectAsync(args, ct);

//            //await _minio.PutAsync(_bookOpts.BooksBucket, key, ms, data.Length, contentType, ct);
//        }


//        /// <inheritdoc/>
//        public async Task<string> UploadFileAsync(
//            Stream fileStream, string fileName, string contentType,
//            CancellationToken ct = default)
//        {
//            var storageUrl = UploadKey(fileName);

//            await EnsureBucketAsync(_uploadOpts.UploadsBucket, ct);

//            var args = new PutObjectArgs()
//                .WithBucket(_uploadOpts.UploadsBucket)
//                .WithObject(storageUrl)
//                .WithStreamData(fileStream)
//                .WithObjectSize(fileStream.Length)
//                .WithContentType(contentType);

//            await _minioClient.PutObjectAsync(args, ct);



//            //await _minio.PutAsync(
//            //    _uploadOpts.UploadsBucket, storageUrl,
//            //    fileStream, fileStream.Length, contentType, ct);
//            return storageUrl;
//        }

//        /// <inheritdoc/>
//        public async Task DeleteFileAsync(string storageUrl, CancellationToken ct = default)
//        {

//            try
//            {
//                var args = new RemoveObjectArgs()
//                    .WithBucket(_uploadOpts.UploadsBucket)
//                    .WithObject(storageUrl);

//                await _minioClient.RemoveObjectAsync(args, ct);
//            }
//            catch (InvalidObjectNameException)
//            {
//            }


//            //_minio.DeleteAsync(_uploadOpts.UploadsBucket, storageUrl, ct);
//        }


//        public async Task DeleteBookAsync(
//            string bookId, string extension,
//            CancellationToken ct = default)
//        {

//            //await DeleteAsync(_bookOpts.BooksBucket, BookSourceKey(bookId, extension), ct);
//            //await DeleteAsync(_bookOpts.BooksBucket, TocKey(bookId), ct);

//            await RecursiveDelete("books", $"{bookId}/", ct);
//            await RecursiveDelete("images", $"{bookId}/", ct);
//            //await RecursiveDelete($"{bookId}/images/", ct);
//        }

//        public async Task DeleteAsync(
//            string bucketName,
//            string objectName,
//            CancellationToken ct = default)
//        {
//            try
//            {
//                var args = new RemoveObjectArgs()
//                    .WithBucket(bucketName)
//                    .WithObject(objectName);

//                await _minioClient.RemoveObjectAsync(args, ct);
//            }
//            catch (ObjectNotFoundException) //Файл уже удалён (либо его не было изначально) - идемпотентность
//            {
//            }
//        }

//        private async Task RecursiveDelete(string bucket, string prefix, CancellationToken ct)
//        {
//            ListObjectsArgs? listArgs;
//            if (bucket == "books")
//                listArgs = new ListObjectsArgs()
//                .WithBucket(_bookOpts.BooksBucket)
//                .WithPrefix(prefix)
//                .WithRecursive(true);
//            else if (bucket == "images")
//                listArgs = new ListObjectsArgs()
//                 .WithBucket(_bookOpts.PublicImagesBucket)
//                 .WithPrefix(prefix)
//                 .WithRecursive(true);
//            else return;

//            var keys = new List<string>();

//            await foreach (var item in _minioClient.ListObjectsEnumAsync(listArgs, ct))
//                keys.Add(item.Key);

//            foreach (var key in keys)
//            {
//                //await _minio.DeleteAsync(_bookOpts.BooksBucket, key, ct);
//                try
//                {
//                    var args = new RemoveObjectArgs()
//                        .WithBucket(_bookOpts.BooksBucket)
//                        .WithObject(key);

//                    await _minioClient.RemoveObjectAsync(args, ct);
//                }
//                catch (InvalidObjectNameException)
//                {
//                }
//            }
//        }


//        private string BookSourceKey(string bookId, string extension)
//            => $"{bookId}/source{extension}";

//        private string TocKey(string bookId)
//            => $"{bookId}/toc.json";


//        private string ChunkKey(string bookId, string fileName, string type)
//            => type=="toc"? TocKey(bookId) : $"{bookId}/chunks/{fileName}";


//        private string BookImageKey(string bookId, string fileName)
//            => $"{bookId}/{fileName}";


//        private static string UploadKey(string fileName)
//            => $"uploads/{Guid.NewGuid()}_{fileName}";

//        private async Task EnsureBucketAsync(string bucketName, CancellationToken ct)
//        {
//            var existsArgs = new BucketExistsArgs().WithBucket(bucketName);
//            bool exists = await _minioClient.BucketExistsAsync(existsArgs, ct);

//            if (!exists)
//            {
//                var makeArgs = new MakeBucketArgs().WithBucket(bucketName);
//                await _minioClient.MakeBucketAsync(makeArgs, ct);
//            }
//        }
//    }
//}


using System.Text;
using Chronolibris.Domain.Interfaces.Services;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Chronolibris.Infrastructure.DataAccess.Files
{
    public sealed class StorageService : IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly BookStorageOptions _bookOpts;
        private readonly UploadStorageOptions _uploadOpts;

        /// <summary>Имя бакета с публичными изображениями (обложки).</summary>
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


        /// <summary>
        /// Сохраняет обложку в PublicImagesBucket по ключу covers/{bookId}/{fileName}.
        /// </summary>
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

        /// <summary>
        /// Удаляет объект из указанного бакета. Идемпотентно.
        /// </summary>
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

        /// <summary>covers/{bookId}/{fileName}  →  напр. covers/42/cover.jpg</summary>
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