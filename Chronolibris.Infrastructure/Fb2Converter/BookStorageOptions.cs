using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Infrastructure.DataAccess.Fb2Converter
{
    /// <summary>
    /// Настройки хранилища книжных фрагментов в MinIO.
    /// Привязывается к секции "BookStorageOptions" в appsettings.json.
    ///
    /// Пример конфига:
    /// <code>
    /// "BookStorageOptions": {
    ///   "BucketName": "books",
    ///   "Prefix": "v1"
    /// }
    /// </code>
    /// </summary>
    public sealed class BookStorageOptions
    {
        public const string SectionName = "BookStorageOptions";

        /// <summary>Имя бакета в MinIO для книжных фрагментов.</summary>
        public string BucketName { get; set; } = "books";

        /// <summary>
        /// Необязательный префикс пути внутри бакета.
        /// Итоговый путь объекта: {Prefix}/{bookId}/{fileName}
        /// Если пустой — путь будет просто {bookId}/{fileName}.
        /// </summary>
        public string Prefix { get; set; } = "";
    }
}
