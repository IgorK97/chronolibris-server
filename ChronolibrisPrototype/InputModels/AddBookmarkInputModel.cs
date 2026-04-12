namespace ChronolibrisWeb.InputModels
{
    public record AddBookmarkInputModel(long bookFileId, string? noteText, int paraIndex);
}
