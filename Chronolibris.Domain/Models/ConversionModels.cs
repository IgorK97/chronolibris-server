using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models
{
    public class ConversionResult
    {
        public required string BookId { get; init; }
        public int TotalElements { get; init; }
        public required StoredFileInfo TocFile { get; init; }
        public required List<StoredFileInfo> PartFiles { get; init; }
        public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
    }

    public class StoredFileInfo
    {
        public required string BookId { get; init; }
        public required string FileName { get; init; }
        public required StoredFileType FileType { get; init; }
        public int GlobalStart { get; init; }
        public int GlobalEnd { get; init; }
        public int[]? XpStart { get; init; }
        public int[]? XpEnd { get; init; }
        public long SizeBytes { get; init; }
    }

    public enum StoredFileType
    {
        Toc,
        Part
    }

    //У всех файлов фрагментов одинаковые названия, поэтому внутрь поместил BookMeta
    public class BookMeta
    {
        [JsonPropertyName("Title")]
        public string? Title { get; init; }

        [JsonPropertyName("ID")]
        public string? Id { get; set; }
    }
}
