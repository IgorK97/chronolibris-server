using Chronolibris.Application.Handlers.Reviews;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests.Reviews;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Models;
using FluentAssertions;
using Moq;

namespace ChronolibrisServer.Tests.Reviews
{
    public class CreateReviewHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IReviewRepository> _reviewRepoMock = new();
        private readonly Mock<IBookRepository> _bookRepoMock = new();
        private readonly Mock<IIdentityService> _identityServiceMock = new();

        private readonly long userId = 1;
        private readonly long bookId = 1;

        public CreateReviewHandlerTests()
        {
            _identityServiceMock.Setup(i => i.IsUserActiveAsync(It.IsAny<long>())).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Books).Returns(_bookRepoMock.Object);
        }

        private CreateReviewCommand BuildCommand(string? text = "Отличная книга",
            short score = 5) => new(UserId : userId,
            BookId : bookId,
            ReviewText : text,
            Score : score
        );

        private void SetupNoExistingReview() =>
        _reviewRepoMock
            .Setup(r => r.GetActiveByUserAndBookAsync(userId, bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReviewDetailsWithVote?)null);

        private void SetupBook(bool isReviewable = true, bool isAvailable = true) =>
            _bookRepoMock
                .Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = bookId,
                    IsReviewable = isReviewable, 
                    IsAvailable = isAvailable,
                Title="Title",
                Description="Description",
                CountryId=1,
                LanguageId=1,
                CreatedAt=DateTime.UtcNow,
                CoverPath=""});

        private CreateReviewHandler CreateHandler() =>
            new(_unitOfWorkMock.Object, _identityServiceMock.Object);

        [Fact]
        public async Task Handle_ExistingReview_ThrowsConflict()
        {
            _reviewRepoMock
                .Setup(r => r.GetActiveByUserAndBookAsync(userId, bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ReviewDetailsWithVote());

            var act = () => CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ChronolibrisException>()
                .Where(e => e.ErrorType == ErrorType.Conflict);

            _bookRepoMock.Verify(
                r => r.GetByIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_BookNotAvailable_ThrowsNotFound()
        {
            SetupNoExistingReview();
            SetupBook(isReviewable: true, isAvailable: false);

            var act = () => CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ChronolibrisException>()
                .Where(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_BookNotReviewable_ThrowsNotFound()
        {
            SetupNoExistingReview();
            SetupBook(isReviewable: false, isAvailable: true);

            var act = () => CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ChronolibrisException>()
                .Where(e => e.ErrorType == ErrorType.NotFound);
        }

    }
}
