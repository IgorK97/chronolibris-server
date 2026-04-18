using System.ComponentModel.DataAnnotations;
using Chronolibris.Domain.Models;

namespace ChronolibrisWeb.InputModels
{
    public class YearRangeAttribute : RangeAttribute
    {
        public YearRangeAttribute(int minYear)
            : base(minYear, DateTime.UtcNow.Year) { }
    }
    public class CreateBookInputModel
    {
        [Required(ErrorMessage = "Название книги отсутствует")]
        [MaxLength(500, ErrorMessage ="Максимальная длина названия - 500 символов")]
        public string Title { get; init; } = string.Empty;
        [Required(ErrorMessage = "Описание книги отсутствует")]
        [MaxLength(5000, ErrorMessage = "Максимальная длина описания - 5000 символов")]
        [MinLength(120, ErrorMessage = "Минимальная длина описания - 120 символов")]
        public string Description { get; init; } = string.Empty;
        [Required(ErrorMessage = "Указание страны обязательно")]
        public int CountryId { get; init; }
        [Required(ErrorMessage = "Название языка обязательно")]
        public int LanguageId { get; init; }
        [YearRange(-10000, ErrorMessage = "Год в пределах от {1} до {2}")]
        public int? Year { get; init; }
        [RegularExpression(@"(?:(?=(?:[^0-9]*[0-9]){10}(?:(?:[^0-9]*[0-9]){3})?$)[\d-]+)?$",
            ErrorMessage ="Неверный формат ISBN")]
        public string? ISBN { get; init; }
        [RegularExpression(@"^[\d\p{L}[\]()+:/=""'*.]{0,500}$",
            ErrorMessage = "Неверный формат ББК")]
        public string? Bbk { get; init; }
        [RegularExpression(@"^[\d\p{L}[\]()+:/=""'*.]{0,500}$",
            ErrorMessage = "Неверный формат УДК")]
        public string? Udk { get; init; }
        [RegularExpression(@"^[\d\s\p{L};/\\:?&=%#[\]\-.,—№§]{0,500}$",
            ErrorMessage = "Неверный формат указания источника")]
        public string? Source { get; init; }
        [Required(ErrorMessage = "Обложка обязательна")]
        public string? CoverBase64 { get; init; } = string.Empty;
        public string? CoverContentType { get; init; } = "image/jpeg";
        public string? CoverFileName { get; init; } = "cover";
        public bool IsAvailable { get; init; } = true;
        public bool IsReviewable { get; init; }
        public int? PublisherId { get; init; }
        public List<PersonRoleFilter>? PersonFilters { get; init; }
    }
    public class UpdateBookInputModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Название книги отсутствует")]
        [MaxLength(500, ErrorMessage = "Максимальная длина названия - 500 символов")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Описание книги отсутствует")]
        [MaxLength(5000, ErrorMessage = "Максимальная длина описания - 5000 символов")]
        [MinLength(120, ErrorMessage = "Минимальная длина описания - 120 символов")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Указание страны обязательно")]
        public int? CountryId { get; set; }
        [Required(ErrorMessage = "Название языка обязательно")]
        public int? LanguageId { get; set; }
        public bool IsAvailable { get; set; } //протестировать без рекуиред,
        //с ним, с атрибутом рекуиред, с атрибутом джисон рекуиред, а также с нуллабле типом
        //(то же самое)
        public bool IsReviewable { get; set; }
        [YearRange(-10000, ErrorMessage = "Год в пределах от {1} до {2}")]
        public int? Year { get; set; }
        public bool YearProvided { get; set; }
        [RegularExpression(@"(?:(?=(?:[^0-9]*[0-9]){10}(?:(?:[^0-9]*[0-9]){3})?$)[\d-]+)?$",
            ErrorMessage = "Неверный формат ISBN")]
        public string? ISBN { get; set; }
        public bool IsbnProvided { get; set; }
        [RegularExpression(@"^[\d\p{L}[\]()+:/=""'*.]{0,500}$",
            ErrorMessage = "Неверный формат ББК")]
        public string? Bbk { get; set; }
        public bool BbkProvided { get; set; }
        [RegularExpression(@"^[\d\p{L}[\]()+:/=""'*.]{0,500}$",
            ErrorMessage = "Неверный формат УДК")]
        public string? Udk { get; set; }
        public bool UdkProvided { get; set; }
        [RegularExpression(@"^[\d\s\p{L};/\\:?&=%#[\]\-.,—№§]{0,500}$",
            ErrorMessage = "Неверный формат указания источника")]
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
