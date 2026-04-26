using System.ComponentModel.DataAnnotations;

namespace ChronolibrisWeb.InputModels
{
    public record AddBookmarkInputModel(long bookFileId, string? noteText,
        [RegularExpression(@"^(/\d+)+$")]
        string Xpointer,
        [MaxLength(200)]
        string Context);
}
