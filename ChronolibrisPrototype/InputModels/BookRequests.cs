using System.ComponentModel.DataAnnotations;
using Chronolibris.Domain.Models;

namespace ChronolibrisWeb.InputModels
{
    public class CreateBookInputModel
    {
        [Required(ErrorMessage = "Название книги отсутствует")]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Описание книги отсутствует")]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public int LanguageId { get; set; }
        public int? Year { get; set; }
        public string? ISBN { get; set; }
        public string? Bbk { get; set; }
        public string? Udk { get; set; }
        public string? Source { get; set; }
        [Required(ErrorMessage = "Обложка обязательна")]
        public string CoverBase64 { get; set; } = string.Empty;
        public string CoverContentType { get; set; } = "image/jpeg";
        public string CoverFileName { get; set; } = "cover";
        public bool IsAvailable { get; set; } = true;
        public bool IsReviewable { get; set; }
        public int? PublisherId { get; set; }
        public List<PersonRoleFilter>? PersonFilters { get; set; }
        public List<int>? ThemeIds { get; set; }
    }
    public class UpdateBookInputModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Название книги отсутствует")]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Описание книги отсутствует")]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        public int? CountryId { get; set; }
        public int? LanguageId { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsReviewable { get; set; }


        public int? Year { get; set; }
        public bool YearProvided { get; set; }

        public string? ISBN { get; set; }
        public bool IsbnProvided { get; set; }

        public string? Bbk { get; set; }
        public bool BbkProvided { get; set; }

        public string? Udk { get; set; }
        public bool UdkProvided { get; set; }

        public string? Source { get; set; }
        public bool SourceProvided { get; set; }

        public int? PublisherId { get; set; }
        public bool PublisherIdProvided { get; set; }

        public string? CoverBase64 { get; set; }
        public string? CoverContentType { get; set; }
        public string? CoverFileName { get; set; }

        public List<PersonRoleFilter>? PersonFilters { get; set; }
        public List<int>? ThemeIds { get; set; }
    }
}
