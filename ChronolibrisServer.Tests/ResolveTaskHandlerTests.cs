using Chronolibris.Application.Handlers.Reports;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using FluentAssertions;
using Moq;

namespace ChronolibrisServer.Tests.Reports
{
    public class ResolveTaskHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IModerationTasksRepository> _taskRepoMock;
        private readonly Mock<ICommentRepository> _commentRepoMock;
        private readonly Mock<IReviewRepository> _reviewRepoMock;
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly Mock<ITransaction> _transactionMock;

        private readonly long moderatorId = 1;
        private readonly long taskId = 1;
        private readonly long targetId = 1;

        public ResolveTaskHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _taskRepoMock = new Mock<IModerationTasksRepository>();
            _commentRepoMock = new Mock<ICommentRepository>();
            _transactionMock = new Mock<ITransaction>();
            _reviewRepoMock = new Mock<IReviewRepository>();
            _bookRepoMock = new Mock<IBookRepository>();
            SetupMocks();
        }

        private void SetupMocks()
        {
            _unitOfWorkMock.Setup(u =>
                u.ModerationTasks).Returns(_taskRepoMock.Object);
            _unitOfWorkMock.Setup(u =>
                u.Comments).Returns(_commentRepoMock.Object);
            _unitOfWorkMock.Setup(u =>
                u.Reviews).Returns(_reviewRepoMock.Object);
            _unitOfWorkMock.Setup(u =>
                u.Books).Returns(_bookRepoMock.Object);

            _unitOfWorkMock.Setup(u =>
            u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_transactionMock.Object));

            _transactionMock.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _transactionMock.Setup(t => t.DisposeAsync())
                .Returns(ValueTask.CompletedTask);
        }

        private ResolveTaskCommand BuildCommand(bool resolution = true)
        {
            return new ResolveTaskCommand(taskId, resolution, moderatorId, "Test");
        }

        private ModerationTask BuildTask(ReportTargetType targetTypeId = ReportTargetType.Comment)
        {
            if(targetTypeId == ReportTargetType.Book)
            return new ModerationTask()
            {
                Id = taskId,
                ModeratedBy = moderatorId,
                StatusId = 2,
                BookId = targetId,
            };
            else if(targetTypeId == ReportTargetType.Review)
            return new ModerationTask()
            {
                Id = taskId,
                ModeratedBy = moderatorId,
                StatusId = 2,
                ReviewId = targetId,
            };
            else if(targetTypeId == ReportTargetType.Comment)
            return new ModerationTask()
            {
                Id = taskId,
                ModeratedBy = moderatorId,
                StatusId = 2,
                CommentId = targetId,
            };
            else throw new ChronolibrisException("Неверный тип контента", ErrorType.Validation);
        }

        private ResolveTaskCommandHandler CreateHandler()
        {
            return new ResolveTaskCommandHandler(_unitOfWorkMock.Object);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task Handle_WrongStatus_ThrowsValidation(int statusId)//метод, усовие, результат, arrange, act, assert
        {
            var task = BuildTask();
            task.StatusId = statusId;
            _taskRepoMock
            .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
            var act = () => CreateHandler().Handle(BuildCommand(), CancellationToken.None);

            await act.Should().ThrowAsync<ChronolibrisException>()
                .Where(e => e.ErrorType == ErrorType.Validation);
        }

        [Fact]
        public async Task Handle_ResolutionFalse_TaskRejected_ContentUntouched()
        {
            var task = BuildTask();
            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);

            var result = await CreateHandler().Handle(
                BuildCommand(resolution: false), CancellationToken.None);

            result.Success.Should().BeTrue();
            task.StatusId.Should().Be(4); //Отклонены
        }

        [Fact]
        public async Task Handle_ResolutionTrue_CommentTarget_CommentDeleted()
        {
            var task = BuildTask();
            var comment = new Comment { Id = targetId,Text="Text",CreatedAt = DateTime.UtcNow, DeletedAt=null };

            _taskRepoMock
                .Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(task);
            _commentRepoMock
                .Setup(r => r.GetByIdAsync(targetId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(comment);

            var result = await CreateHandler().Handle(BuildCommand(resolution: true), CancellationToken.None);

            result.Success.Should().BeTrue();
            task.StatusId.Should().Be(3); //Приняты
            comment.DeletedAt.Should().NotBeNull();
        }
    }
}