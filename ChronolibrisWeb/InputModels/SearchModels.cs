using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{

    public class SimpleSearchInputModel
    {
        //[Required(ErrorMessage = "Параметр query обязателен")]
        [MaxLength(500, ErrorMessage = "Поисковый запрос слишком длинный")]
        public required string Query { get; set; }

        [Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100")]
        public int PageSize { get; set; } = 20;
        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }
    }

    public class PersonRoleFilterModel
    {
        public long RoleId { get; set; }

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
        [MaxLength(100, ErrorMessage = "Максимальное количество фильтров персоналий — 100")]
        public List<PersonRoleFilterModel> PersonFilters { get; set; } = [];
        public long ThemeId { get; set; }
        public long SelectionId { get; set; }
        [MaxLength(100, ErrorMessage = "Максимальное количество тегов — 100")]
        public List<long> RequiredTagIds { get; set; } = [];
        [MaxLength(100, ErrorMessage = "Максимальное количество тегов — 100")]
        public List<long> ExcludedTagIds { get; set; } = [];

    }
}