using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Application.Models
{
    public class BookFileDto
    {
        public long Id { get; set; }
        public long BookId { get; set; }
        public int FormatId { get; set; }
        public string? FormatName { get; set; }
        public string StorageUrl { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string FileSizeDisplay => FormatFileSize(FileSizeBytes);
        public bool IsReadable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public int Version { get; set; }
        public long BookFileStatusId { get; set; }
        public string? BookFileStatusName { get; set; }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }
    }

}
