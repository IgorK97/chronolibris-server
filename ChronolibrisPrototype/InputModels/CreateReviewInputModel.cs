namespace ChronolibrisWeb.InputModels
{
    public class CreateReviewInputModel
    {
        public long BookId { get; init; }
        public string? ReviewText { get; init; }
        public short Score { get; init; }
    }
}
