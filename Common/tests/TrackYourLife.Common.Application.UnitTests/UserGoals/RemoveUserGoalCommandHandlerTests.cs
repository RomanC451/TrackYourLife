using Moq;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.UserGoals.Commands.RemoveUserGoal;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.UserGoals;
using TrackYourLife.Common.Domain.Users.StrongTypes;

namespace TrackYourLife.Common.Application.UnitTests.UserGoals;

public sealed class RemoveUserGoalCommandHandlerTests
{
    private readonly Mock<IUserGoalRepository> _userGoalRepositoryMock = new();
    private readonly Mock<IUserIdentifierProvider> _userIdentifierProviderMock = new();

    private readonly RemoveUserGoalCommandHandler _handler;

    public RemoveUserGoalCommandHandlerTests()
    {
        _handler = new RemoveUserGoalCommandHandler(
            _userGoalRepositoryMock.Object,
            _userIdentifierProviderMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new RemoveUserGoalCommand(Id: UserGoalId.NewId());
        _userGoalRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserGoal)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.UserGoal.NotFound(command.Id).Code, result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserGoalNotOwned_ShouldReturnFailure()
    {
        // Arrange
        var command = new RemoveUserGoalCommand(Id: UserGoalId.NewId());
        var userGoal = UserGoal
            .Create(
                id: command.Id,
                userId: UserId.NewId(),
                startDate: DateOnly.FromDateTime(DateTime.UtcNow),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
                type: UserGoalType.Calories,
                value: 300,
                perPeriod: UserGoalPerPeriod.Month
            )
            .Value;
        _userGoalRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userGoal);
        _userIdentifierProviderMock.Setup(x => x.UserId).Returns(UserId.NewId());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.UserGoal.NotOwned(command.Id).Code, result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenUserGoalFoundAndOwned_ShouldRemoveUserGoal()
    {
        // Arrange
        var command = new RemoveUserGoalCommand(Id: UserGoalId.NewId());

        var userGoal = UserGoal
            .Create(
                id: command.Id,
                userId: UserId.NewId(),
                startDate: DateOnly.FromDateTime(DateTime.UtcNow),
                endDate: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1),
                type: UserGoalType.Calories,
                value: 300,
                perPeriod: UserGoalPerPeriod.Month
            )
            .Value;
        _userGoalRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userGoal);
        _userIdentifierProviderMock.Setup(x => x.UserId).Returns(userGoal.UserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _userGoalRepositoryMock.Verify(x => x.Remove(userGoal), Times.Once);
    }
}
