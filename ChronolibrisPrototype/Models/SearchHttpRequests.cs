using System.ComponentModel.DataAnnotations;

namespace Chronolibris.API.Controllers.Search
{
    // ── Простой поиск ─────────────────────────────────────────────────────────
    // GET /api/search?query=война&pageSize=20
    // Курсор первой страницы: lastBestSimilarity и lastId отсутствуют.
    // Курсор следующих страниц: берётся из полей LastBestSimilarity и LastId ответа.

    public class SimpleSearchHttpRequest
    {
        [Required(ErrorMessage = "Параметр query обязателен")]
        //[MinLength(1, ErrorMessage = "Поисковый запрос не может быть пустым")]
        [MaxLength(200, ErrorMessage = "Поисковый запрос слишком длинный")]
        public required string Query { get; set; }

        [Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100")]
        public int PageSize { get; set; } = 20;

        // Составной keyset-курсор. Оба поля null = первая страница.
        // Клиент передаёт значения из предыдущего ответа (LastBestSimilarity + LastId).
        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }
    }

    // ── Расширенный поиск ─────────────────────────────────────────────────────
    // POST /api/search/advanced
    // Тело — JSON. Курсор передаётся в теле вместе с фильтрами.

    public class PersonRoleFilterHttpRequest
    {
        [Required]
        public long RoleId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Укажите хотя бы одну персоналию")]
        public List<long> PersonIds { get; set; } = [];
    }

    public class AdvancedSearchInputModel
    {
        //[Required(ErrorMessage = "Параметр query обязателен")]
        //[MinLength(1)]
        [MaxLength(200)]
        public string? Query { get; set; }

        [Range(1, 100)]
        public int PageSize { get; set; } = 20;

        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }

        public List<PersonRoleFilterHttpRequest> PersonFilters { get; set; } = [];
        public long ThemeId { get; set; }
        public long SelectionId { get; set; }
        public List<long> RequiredTagIds { get; set; } = [];
        public List<long> ExcludedTagIds { get; set; } = [];

    }
}