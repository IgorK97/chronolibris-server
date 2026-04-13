using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Models
{
   
    public class CreateContentRequest
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long CountryId { get; set; }

        [Required]
        public long ContentTypeId { get; set; }

        [Required]
        public long LanguageId { get; set; }

        public int? Year { get; set; }
        public List<PersonRoleFilter> PersonFilters { get; set; } = new();
        public List<long> ThemeIds { get; set; } = new();
    }

    public class UpdateContentRequest : IRequest<Unit>
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public long? CountryId { get; set; }

        public long? ContentTypeId { get; set; }

        public long? LanguageId { get; set; }

        public int? Year { get; set; }
        public bool YearProvided { get; set; }
        public List<PersonRoleFilter>? PersonFilters { get; set; }
        public List<long>? ThemeIds { get; set; }
        public List<long>? TagIds { get; set; }
    }
    public class ContentListResponse
    {
        public List<ContentDto> Items { get; set; } = new();
        public string? NextCursor { get; set; }
        public string? PrevCursor { get; set; }
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }
}