using Moq;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.UserGoals.Queries.GetActiveUserGoalByType;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.UserGoals;
using TrackYourLife.Common.Domain.Users.StrongTypes;
using TrackYourLife.Contracts.UserGoals;

namespace TrackYourLife.Common.Application.UnitTests.UserGoals;

public class GetActiveUserGoalByTypeQueryHandlerTests
{
    private readonly Mock<IUserGoalQuery> _userGoalQuery = new();
    private readonly Mock<IUserIdentifierProvider> _userIdentifierProvider = new();
    private readonly GetActiveUserGoalByTypeQueryHandler _sut;

    public GetActiveUserGoalByTypeQueryHandlerTests()
    {
        _sut = new GetActiveUserGoalByTypeQueryHandler(
            _userGoalQuery.Object,
            _userIdentifierProvider.Object
        );
    }

    private UserGoal? _activeUserGoal = null;

    private void SetupDependencies()
    {
        _userGoalQuery
            .Setup(
                x =>
                    x.GetActiveGoalByTypeAsync(
                        It.IsAny<UserId>(),
                        It.IsAny<UserGoalType>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(_activeUserGoal);
    }

    private static UserGoal CreateGoal(
        int value,
        string startDate,
        string? endDate,
        UserGoalId? id = null,
        UserId? userId = null
    ) =>
        UserGoal
            .Create(
                id: id ?? UserGoalId.NewId(),
                userId: userId ?? UserId.NewId(),
                type: UserGoalType.Calories,
                value: value,
                perPeriod: UserGoalPerPeriod.Day,
                startDate: DateOnly.Parse(startDate),
                endDate: !string.IsNullOrEmpty(endDate) ? DateOnly.Parse(endDate) : null
            )
            .Value;

    [Fact]
    public async Task Handle_WhenUserGoalIsNotFound_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(userId);

        SetupDependencies();

        // Act
        var result = await _sut.Handle(
            new GetActiveUserGoalByTypeQuery(UserGoalType.Water),
            CancellationToken.None
        );

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.UserGoal.NotExisting(UserGoalType.Water), result.Error);

        _userGoalQuery.Verify(
            repo =>
                repo.GetActiveGoalByTypeAsync(
                    It.IsAny<UserId>(),
                    It.IsAny<UserGoalType>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalIsFound_ShouldReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var userGoal = CreateGoal(2000, "2022-01-01", "2022-01-31", userId: userId);

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(userId);

        _activeUserGoal = userGoal;

        SetupDependencies();

        // Act
        var result = await _sut.Handle(
            new GetActiveUserGoalByTypeQuery(UserGoalType.Calories),
            CancellationToken.None
        );

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(
            new UserGoalResponse(
                userGoal.Id,
                userGoal.Type,
                userGoal.Value,
                userGoal.PerPeriod,
                userGoal.StartDate,
                userGoal.EndDate
            ),
            result.Value
        );

        _userGoalQuery.Verify(
            repo =>
                repo.GetActiveGoalByTypeAsync(
                    It.IsAny<UserId>(),
                    It.IsAny<UserGoalType>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }
}
