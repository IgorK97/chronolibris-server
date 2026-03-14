using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronolibris.Application.Fb2Converter.Interfaces;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Chronolibris.Infrastructure.DataAccess.Fb2Converter
{
    /// <summary>
    /// Реализация <see cref="IBookStorage"/> поверх MinIO SDK.
    ///
    /// Зависимость: пакет Minio (официальный .NET SDK).
    /// <code>
    /// dotnet add package Minio
    /// </code>
    ///
    /// Пример регистрации в DI:
    /// <code>
    /// builder.Services.AddSingleton&lt;IMinioClient&gt;(sp =>
    ///     new MinioClient()
    ///         .WithEndpoint("localhost", 9000)
    ///         .WithCredentials("minioadmin", "minioadmin")
    ///         .Build());
    ///
    /// builder.Services.AddSingleton&lt;IBookStorage, MinioBookStorage&gt;();
    /// </code>
    /// </summary>
    public sealed class MinioBookStorage : IBookStorage
    {
        // Добавьте using Minio; using Minio.DataModel.Args; когда подключите пакет.
        // Здесь классы MinioClient / PutObjectArgs и т.д. указаны через dynamic-like
        // псевдонимы, чтобы файл компилировался в «заглушечном» окружении.
        // Раскомментируйте using-директивы и уберите #if-заглушку после добавления пакета.

        // ── Configuration ────────────────────────────────────────────────────────

        /// <summary>Имя бакета в MinIO, куда кладутся все книги.</summary>
        public string BucketName { get; }

        /// <summary>
        /// Префикс пути внутри бакета.
        /// Итоговый путь объекта: {Prefix}/{bookId}/{fileName}
        /// </summary>
        public string Prefix { get; }

        // ── Internals ─────────────────────────────────────────────────────────────
        private readonly IMinioClient _minioClient; // на самом деле IMinioClient

        /// <param name="minioClient">Экземпляр IMinioClient (Minio SDK).</param>
        /// <param name="bucketName">Имя бакета.</param>
        /// <param name="prefix">Необязательный префикс пути, по умолчанию "books".</param>
        public MinioBookStorage(IMinioClient minioClient,
                                string bucketName = "books",
                                string prefix = "")
        {
            _minioClient = minioClient ?? throw new ArgumentNullException(nameof(minioClient));
            BucketName = bucketName;
            Prefix = prefix;
        }

        // ── IBookStorage ──────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public async Task SaveAsync(string bookId, string fileName, string content,
            CancellationToken cancellationToken = default)
        {
            var objectName = BuildObjectName(bookId, fileName);
            var bytes = Encoding.UTF8.GetBytes(content);
            using var ms = new MemoryStream(bytes);




            await EnsureBucketAsync(cancellationToken);

            var putArgs = new PutObjectArgs()
                 .WithBucket(BucketName)
                 .WithObject(objectName)
                 .WithStreamData(ms)
                 .WithObjectSize(bytes.Length)
                 .WithContentType("application/json; charset=utf-8");

            await ((IMinioClient)_minioClient).PutObjectAsync(putArgs, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<string?> ReadAsync(string bookId, string fileName,
            CancellationToken cancellationToken = default)
        {
            var objectName = BuildObjectName(bookId, fileName);

            string? result = null;
            var getArgs = new GetObjectArgs()
                 .WithBucket(BucketName)
                 .WithObject(objectName)
                .WithCallbackStream(async (stream, ct) =>
                {
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    result = await reader.ReadToEndAsync(ct);
                });


            await ((IMinioClient)_minioClient).GetObjectAsync(getArgs, cancellationToken);
            return result;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(string bookId, string fileName,
            CancellationToken cancellationToken = default)
        {
            var objectName = BuildObjectName(bookId, fileName);


            try
            {
                var statArgs = new StatObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(objectName);
                await ((IMinioClient)_minioClient).StatObjectAsync(statArgs, cancellationToken);
                return true;
            }
            catch (ObjectNotFoundException)
            {
                return false;
            }
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private string BuildObjectName(string bookId, string fileName)
        {
            return string.IsNullOrEmpty(Prefix)
                ? $"{bookId}/{fileName}"
                : $"{Prefix}/{bookId}/{fileName}";
        }


        private async Task EnsureBucketAsync(CancellationToken ct)
        {
            var beArgs = new BucketExistsArgs().WithBucket(BucketName);
            bool exists = await ((IMinioClient)_minioClient).BucketExistsAsync(beArgs, ct);
            if (!exists)
            {
                var mbArgs = new MakeBucketArgs().WithBucket(BucketName);
                await ((IMinioClient)_minioClient).MakeBucketAsync(mbArgs, ct);
            }
        }

        public async Task SaveImageAsync(string bookId, string fileName, byte[] data,
    string contentType, CancellationToken cancellationToken = default)
        {
            await EnsureBucketAsync(cancellationToken);

            var objectName = BuildObjectName(bookId, fileName);
            using var ms = new MemoryStream(data);

            var args = new PutObjectArgs()
                .WithBucket(BucketName)
                .WithObject(objectName)
                .WithStreamData(ms)
                .WithObjectSize(data.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(args, cancellationToken);
        }

        public async Task<Stream?> ReadStreamAsync(string objectName,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var ms = new MemoryStream();

                var args = new GetObjectArgs()
                    .WithBucket(BucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(async (stream, ct) =>
                    {
                        await stream.CopyToAsync(ms, ct);
                    });

                await _minioClient.GetObjectAsync(args, cancellationToken);

                ms.Position = 0;
                return ms;
            }
            catch (ObjectNotFoundException)
            {
                return null;
            }
        }



    }

}
