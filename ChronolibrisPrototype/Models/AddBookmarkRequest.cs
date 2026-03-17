namespace ChronolibrisPrototype.Models
{
    public record AddBookmarkRequest(long bookFileId, string? noteText, int paraIndex);
}
