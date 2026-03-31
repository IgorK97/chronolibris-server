using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models.Search
{
   public class PersonRoleFilter
    {
        public long RoleId { get; set; }
        public List<long> PersonIds { get; set; } = [];

    }



    //similarity или ilike (простой поиск по названию),
    //пагинация офсетная
    public class SimpleSearchOffsetRequest
    {
        public required string Query { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public long? UserId { get; set; }
    }



    //similarity или ilike (сложный, расширенный поиск),
    //пагинация офсетная
    public class AdvancedSearchOffsetRequest
    {
        public required string Query { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public List<PersonRoleFilter> PersonFilters { get; set; } = [];
        public List<long> RequiredThemeIds { get; set; } = [];
        public List<long> ExcludedThemeIds { get; set; } = [];
        public List<long> RequiredTagIds { get; set; } = [];
        public List<long> ExcludedTagIds { get; set; } = [];

        public List<long> PublisherIds { get; set; } = [];
        public List<long> LanguageIds { get; set; } = [];
        public List<long> CountryIds { get; set; } = [];
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public long? UserId { get; set; }

    }


    public class SimpleSearchKeysetRequest
    {
        public required string Query { get; set; }
        public int PageSize { get; set; } = 20;
        public long? UserId { get; set; }

        // Курсор — null означает первую страницу
        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }
    }

    public class AdvancedSearchKeysetRequest
    {
        public string? Query { get; set; }
        public int PageSize { get; set; } = 20;

        public List<PersonRoleFilter> PersonFilters { get; set; } = [];
        public long ThemeId { get; set; }
        public required long SelectionId { get; set; }
        //public List<long> RequiredThemeIds { get; set; } = [];
        //public List<long> ExcludedThemeIds { get; set; } = [];
        public List<long> RequiredTagIds { get; set; } = [];
        public List<long> ExcludedTagIds { get; set; } = [];

        //public List<long> PublisherIds { get; set; } = [];
        //public List<long> LanguageIds { get; set; } = [];
        //public List<long> CountryIds { get; set; } = [];
        //public int? YearFrom { get; set; }
        //public int? YearTo { get; set; }
        public long? UserId { get; set; }

        public double? LastBestSimilarity { get; set; }
        public long? LastId { get; set; }
    }

}
