using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Models.Search
{
    //public class LanguageDto
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //}

    //public class CountryDto
    //{
    //    public long Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //}


    public class PersonRoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public required int Kind { get; set; }
    }


    public class PersonSuggestionDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }


    public class TagSuggestionDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MatchedName { get; set; }
        public long? TagTypeId { get; set; }
        public string? TagTypeName { get; set; }
        public bool? HasChildren { get; set; }
    }
}
