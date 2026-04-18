using System.Linq.Expressions;
using Chronolibris.Application.Handlers.Bookmarks;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests.Bookmarks;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using FluentAssertions;
using Moq;
namespace ChronolibrisServer.Tests.Bookmarks;
public class AddBookmarkHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IIdentityService> _identityMock;
    private readonly Mock<ITransaction> _transactionMock; 
    private readonly AddBookmarkHandler _handler;

    public AddBookmarkHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _identityMock = new Mock<IIdentityService>();
        _transactionMock = new Mock<ITransaction>();

        _uowMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_transactionMock.Object);

        _handler = new AddBookmarkHandler(_uowMock.Object, _identityMock.Object);
    }

    [Fact]
    public async Task Handle_Success_ShouldReturnResult()
    {
        var command = new AddBookmarkCommand(UserId: 1, BookFileId: 10, ParaIndex: 5, NoteText: "Закладка");
        var bookFile = new BookFile { MaxParaIndex = 100, Book = new Book { IsAvailable = true } };

        _identityMock.Setup(s => s.IsUserActiveAsync(command.UserId)).ReturnsAsync(true);
        _uowMock.Setup(u => u.BookFiles.GetByIdAsync(command.BookFileId, It.IsAny<CancellationToken>())).ReturnsAsync(bookFile);
        _uowMock.Setup(u => u.Bookmarks.CountAsync(It.IsAny<Expression<Func<Bookmark, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        _uowMock.Verify(u => u.Bookmarks.AddAsync(It.IsAny<Bookmark>(), It.IsAny<CancellationToken>()), Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_LimitReached_ShouldThrowException()
    {
        var command = new AddBookmarkCommand(UserId: 1, BookFileId: 10, ParaIndex: 5, NoteText:"");
        var bookFile = new BookFile { MaxParaIndex = 100, Book = new Book { IsAvailable = true } };

        _identityMock.Setup(s => s.IsUserActiveAsync(command.UserId)).ReturnsAsync(true);
        _uowMock.Setup(u => u.BookFiles.GetByIdAsync(command.BookFileId, It.IsAny<CancellationToken>())).ReturnsAsync(bookFile);

        _uowMock.Setup(u => u.Bookmarks.CountAsync(It.IsAny<Expression<Func<Bookmark, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(500);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ChronolibrisException>();
    }

    [Fact]
    public async Task Handle_IndexOutOfBounds_ShouldThrowException()
    {
        var command = new AddBookmarkCommand(UserId: 1, BookFileId: 10, ParaIndex: 999, NoteText: "");
        var bookFile = new BookFile { MaxParaIndex = 100, Book = new Book { IsAvailable = true } };

        _identityMock.Setup(s => s.IsUserActiveAsync(command.UserId)).ReturnsAsync(true);
        _uowMock.Setup(u => u.BookFiles.GetByIdAsync(command.BookFileId, It.IsAny<CancellationToken>())).ReturnsAsync(bookFile);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ChronolibrisException>();
    }
}