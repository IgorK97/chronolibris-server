using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{

    public class SimpleSearchInputModel
    {
        [Required(ErrorMessage = "Параметр query обязателен")]
        [MaxLength(500, ErrorMessage = "Поисковый запрос слишком длинный")]
        public required string Query { get; set; }

        [Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100")]
        public int PageSize { get; set; } = 20;
        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }
    }

    public class PersonRoleFilterModel
    {
        [Required]
        public long RoleId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Укажите хотя бы одну персоналию")]
        public List<long> PersonIds { get; set; } = [];
    }

    public class AdvancedSearchInputModel
    {
        [MaxLength(200)]
        public string? Query { get; set; }

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }

        public List<PersonRoleFilterModel> PersonFilters { get; set; } = [];
        public long ThemeId { get; set; }
        public long SelectionId { get; set; }
        public List<long> RequiredTagIds { get; set; } = [];
        public List<long> ExcludedTagIds { get; set; } = [];

    }
}