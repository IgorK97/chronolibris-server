using Chronolibris.Application.Handlers.Reports;
using Chronolibris.Application.Interfaces;
using Chronolibris.Application.Requests.Reports;
using Chronolibris.Domain.Entities;
using Chronolibris.Domain.Exceptions;
using Chronolibris.Domain.Interfaces.Repository;
using Chronolibris.Domain.Options;
using FluentAssertions;
using Moq;

namespace ChronolibrisServer.Tests.Reports;

public class CreateReportCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IReportRepository> _reportRepoMock = new();
    private readonly Mock<IModerationTasksRepository> _tasksRepoMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<ITransaction> _transactionMock = new();


    private const int UserId = 1;
    private const int TargetId = 10;
    private const int TargetTypeId = 2;
    private const int ReasonTypeId = 3;

    private readonly ReportingOptions _defaultOptions = new()
    {
        ReportCooldown = TimeSpan.FromDays(1)
    };

    public CreateReportCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Reports).Returns(_reportRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ModerationTasks).Returns(_tasksRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _identityServiceMock.Setup(i => i.IsUserActiveAsync(It.IsAny<long>())).ReturnsAsync(true);
        _transactionMock
            .Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _transactionMock
            .Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _transactionMock
            .Setup(t => t.DisposeAsync())
            .Returns(ValueTask.CompletedTask);
        _unitOfWorkMock
           .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(_transactionMock.Object);

        _tasksRepoMock
            .Setup(r => r.GetActiveByTarget(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModerationTask?)null);
    }

    private CreateReportCommandHandler CreateHandler(ReportingOptions? options = null)
        => new(_unitOfWorkMock.Object, options ?? _defaultOptions, _identityServiceMock.Object);

    private CreateReportCommand BuildCommand(
        long userId = UserId,
        int targetId = TargetId,
        int targetTypeId = TargetTypeId,
        int reasonTypeId = ReasonTypeId,
        string description = "Спам")
        => new(targetId, targetTypeId, reasonTypeId, description, userId);

    [Fact]
    public async Task Handle_WithActiveTask_SetsTaskIdOnReport()
    {
        var activeTask = new ModerationTask { Id = 99 };

        _reportRepoMock
            .Setup(r => r.GetLastUserReport(UserId, TargetTypeId, TargetId, ReasonTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report?)null);

        _tasksRepoMock
            .Setup(r => r.GetActiveByTarget(TargetId, TargetTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeTask);

        Report? savedReport = null;
        _reportRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .Callback<Report, CancellationToken>((r, _) => savedReport = r);


        var result = await CreateHandler().Handle(BuildCommand(), CancellationToken.None);

        savedReport.Should().NotBeNull();
        savedReport.ModerationTaskId.Should().Be(99);
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithoutActiveTask_LeavesTaskIdNull()
    {
        _reportRepoMock
            .Setup(r => r.GetLastUserReport(UserId, TargetTypeId, TargetId, ReasonTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Report?)null);

        _tasksRepoMock
            .Setup(r => r.GetActiveByTarget(TargetId, TargetTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ModerationTask?)null);

        Report? savedReport = null;
        _reportRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Report>(), It.IsAny<CancellationToken>()))
            .Callback<Report, CancellationToken>((r, _) => savedReport = r);

        var result = await CreateHandler().Handle(BuildCommand(), CancellationToken.None);

        savedReport!.ModerationTaskId.Should().BeNull();
        result.Success.Should().BeTrue();

    }


    [Fact]
    public async Task Handle_ReportWithinCooldown_ThrowsTooManyRequests()
    {

        var recentReport = new Report
        {
            CreatedAt = DateTime.UtcNow - TimeSpan.FromMinutes(10)
        };

        _reportRepoMock
            .Setup(r => r.GetLastUserReport(UserId, TargetTypeId, TargetId, ReasonTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentReport);

        var act = () => CreateHandler().Handle(BuildCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<ChronolibrisException>()
            .Where(e => e.ErrorType == ErrorType.TooManyRequests);
    }

}