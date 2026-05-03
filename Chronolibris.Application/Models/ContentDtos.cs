using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chronolibris.Domain.Models;
using MediatR;

namespace Chronolibris.Application.Models
{
   
    public class CreateContentRequest
    {
        [MinLength(1)]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [MinLength(120)]
        [MaxLength(5000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public long CountryId { get; set; }

        [Required]
        public long ContentTypeId { get; set; }

        [Required]
        public long LanguageId { get; set; }

        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public List<PersonRoleFilter> PersonFilters { get; set; } = new();
        public List<long> ThemeIds { get; set; } = new();
    }

    public class UpdateContentRequest : IRequest<Unit>
    {
        public long Id { get; set; }
        [MaxLength(500)]
        public string? Title { get; set; }
        [MaxLength(5000)]
        public string? Description { get; set; }

        public long? CountryId { get; set; }

        public long? ContentTypeId { get; set; }

        public long? LanguageId { get; set; }

        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public bool YearFromProvided { get; set; }
        public bool YearToProvided { get; set; }
        public List<PersonRoleFilter>? PersonFilters { get; set; }
        public List<long>? ThemeIds { get; set; }
        public List<long>? TagIds { get; set; }
    }
}