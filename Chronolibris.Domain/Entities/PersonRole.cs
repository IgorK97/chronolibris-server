using System.ComponentModel.DataAnnotations;

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

    public enum PersonRoleKind
    {
        Content = 1,    // роль относится только к содержанию
        Book = 2,       // роль относится только к воплощению
        Both = 3        // роль применима и к содержанию, и к книге
    }
    public class PersonRole
    {
        public required long Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        public PersonRoleKind Kind { get; set; }
    }
}
