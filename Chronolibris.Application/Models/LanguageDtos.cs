using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chronolibris.Domain.Entities;

namespace Chronolibris.Application.Models
{

    public class CreateLanguageRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateLanguageRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}