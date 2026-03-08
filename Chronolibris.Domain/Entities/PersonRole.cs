using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronolibris.Domain.Entities
{
    public enum PersonRoles
    {
        Author = 1,
        Translator = 2,
        Editor = 3,
        Illustrator = 4,
        Compiler = 5,
        Proofreader = 6,
        ScientificEditor = 7,
        LiteraryEditor = 8,
        TechnicalEditor = 9,
        TranslatorEditor = 10,
        Scanner = 11,
        AuthorOfIntroduction = 12,
        AuthorOfAfterword = 13,
        Commentator = 14,
        Designer = 15,

    }
    public class PersonRole
    {
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
    }
}
