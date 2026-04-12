using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Infrastructure.Services.Files
{
    public sealed class MinioOptions
    {
        public string Endpoint { get; set; } = "localhost:9000";
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public bool UseSSL { get; set; } = false;
    }
    public sealed class BookStorageOptions
    {
        public string BooksBucket { get; set; } = "books";
        public string PublicImagesBucket { get; set; } = "images";
        public string CoversBucket { get; set; } = "covers";
    }

    public sealed class UploadStorageOptions
    {
        public string UploadsBucket { get; set; } = "pcovers";
    }
}
