using Moq;
using System.Collections.ObjectModel;
using TrackYourLife.Application.UserGoals.Commands.AddUserGoal;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.UserGoals.Commands.AddUserGoal;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.UserGoals;
using TrackYourLife.Common.Domain.Users.StrongTypes;

namespace TrackYourLife.Common.Application.UnitTests.UserGoals;

public class AddUserGoalCommandHandlerTests
{
    private readonly Mock<IUserGoalRepository> _userGoalRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserIdentifierProvider> _userIdentifierProvider = new();
    private readonly UpdateUserGoalCommandHandler _sut;

    private List<UserGoal> _afterActGoals;

    private ReadOnlyCollection<UserGoal> _beforeActGoals;

    public AddUserGoalCommandHandlerTests()
    {
        _sut = new UpdateUserGoalCommandHandler(
            _userGoalRepository.Object,
            _unitOfWork.Object,
            _userIdentifierProvider.Object
        );

        _afterActGoals = new();
        _beforeActGoals = new(_afterActGoals);

        SetupDependencies();
    }

    private void SetupDependencies()
    {
        _userGoalRepository
            .Setup(
                repo =>
                    repo.GetOverlappingGoalsAsync(
                        It.IsAny<UserGoal>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(_beforeActGoals.ToList());

        _userGoalRepository
            .Setup(repo => repo.AddAsync(It.IsAny<UserGoal>(), It.IsAny<CancellationToken>()))
            .Callback<UserGoal, CancellationToken>((userGoal, _) => _afterActGoals.Add(userGoal));
        _userGoalRepository
            .Setup(repo => repo.Update(It.IsAny<UserGoal>()))
            .Callback<UserGoal>(
                (userGoal) =>
                {
                    var index = _afterActGoals.FindIndex(g => g.Id == userGoal.Id);
                    _afterActGoals[index] = userGoal;
                }
            );

        _userGoalRepository
            .Setup(repo => repo.Remove(It.IsAny<UserGoal>()))
            .Callback<UserGoal>((userGoal) => _afterActGoals.Remove(userGoal));

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(UserId.NewId());
    }

    private void SetExistingGoals(params UserGoal[] goals)
    {
        _afterActGoals.Clear();
        _afterActGoals.AddRange(goals);

        _beforeActGoals = new ReadOnlyCollection<UserGoal>(
            _afterActGoals
                .Select(
                    goal =>
                        UserGoal
                            .Create(
                                id: goal.Id,
                                userId: goal.UserId,
                                type: goal.Type,
                                value: goal.Value,
                                perPeriod: goal.PerPeriod,
                                startDate: goal.StartDate,
                                endDate: goal.EndDate
                            )
                            .Value
                )
                .ToList()
        );

        SetupDependencies();
    }

    private static UserGoal CreateGoal(int value, string startDate, string? endDate) =>
        UserGoal
            .Create(
                id: UserGoalId.NewId(),
                userId: UserId.NewId(),
                type: UserGoalType.Calories,
                value: value,
                perPeriod: UserGoalPerPeriod.Day,
                startDate: DateOnly.Parse(startDate),
                endDate: !string.IsNullOrEmpty(endDate) ? DateOnly.Parse(endDate) : null
            )
            .Value;

    private void VerifyCalls(
        Times GetContainingGoals,
        Times Add,
        Times Remove,
        Times Update,
        Times SaveChanges
    )
    {
        // Verify that the repository methods were called correctly
        _userGoalRepository.Verify(
            repo =>
                repo.GetOverlappingGoalsAsync(It.IsAny<UserGoal>(), It.IsAny<CancellationToken>()),
            GetContainingGoals
        );

        _userGoalRepository.Verify(
            repo => repo.AddAsync(It.IsAny<UserGoal>(), It.IsAny<CancellationToken>()),
            Add
        );

        _userGoalRepository.Verify(repo => repo.Remove(It.IsAny<UserGoal>()), Remove);

        _userGoalRepository.Verify(repo => repo.Update(It.IsAny<UserGoal>()), Update);

        // Verify that the unit of work method was called correctly
        _unitOfWork.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), SaveChanges);
    }

    [Fact]
    public async Task Handle_WithoutOverlappingOldGoals_CreatesNewGoal()
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 10,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse("02-21-2024"),
            EndDate: null,
            Force: false
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(command.Value, _afterActGoals[0].Value);
        Assert.Equal(command.Type, _afterActGoals[0].Type);
        Assert.Equal(command.PerPeriod, _afterActGoals[0].PerPeriod);
        Assert.Equal(command.StartDate, _afterActGoals[0].StartDate);
        Assert.Equal(DateOnly.MaxValue, _afterActGoals[0].EndDate);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Once(),
            Remove: Times.Never(),
            Update: Times.Never(),
            SaveChanges: Times.Once()
        );
    }

    [Theory]
    [InlineData("02-10-2024", "02-29-2024", "02-01-2024", "02-29-2024", "02-01-2024", "02-09-2024")] // new goal fully contained by old goal
    [InlineData("02-01-2024", "02-10-2024", "02-01-2024", "02-29-2024", "02-11-2024", "02-29-2024")]
    [InlineData("02-10-2024", "03-04-2024", "02-01-2024", "02-29-2024", "02-01-2024", "02-09-2024")] // overlapping with start date
    [InlineData("02-10-2024", null, "02-01-2024", "02-29-2024", "02-01-2024", "02-09-2024")]
    [InlineData("02-10-2024", null, "02-01-2024", null, "02-01-2024", "02-09-2024")]
    [InlineData("01-31-2024", "02-04-2024", "02-01-2024", "02-29-2024", "02-05-2024", "02-29-2024")] // overlapping with end date
    [InlineData("02-01-2024", "02-10-2024", "02-01-2024", null, "02-11-2024", "12-31-9999")]
    public async Task Handle_PartialOverlappingOldGoalAndWithForce_UpdatesOverlappingOldGoalAndCreatesNewGoal(
        string newGoalStartDate,
        string? newGoalEndDate,
        string oldGoalStartDate,
        string? oldGoalEndDate,
        string expectedOldGoalStartDate,
        string expectedOldGoalEndDate
    )
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 3000,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse(newGoalStartDate),
            EndDate: newGoalEndDate is null ? null : DateOnly.Parse(newGoalEndDate),
            Force: true
        );

        SetExistingGoals(
            CreateGoal(value: 2000, startDate: oldGoalStartDate, endDate: oldGoalEndDate)
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        Assert.True(result.IsSuccess);
        Assert.Equal(2, _afterActGoals.Count);

        Assert.Equal(_beforeActGoals[0].Value, _afterActGoals[0].Value);
        Assert.Equal(_beforeActGoals[0].Type, _afterActGoals[0].Type);
        Assert.Equal(_beforeActGoals[0].PerPeriod, _afterActGoals[0].PerPeriod);
        Assert.Equal(DateOnly.Parse(expectedOldGoalStartDate), _afterActGoals[0].StartDate);
        Assert.Equal(DateOnly.Parse(expectedOldGoalEndDate), _afterActGoals[0].EndDate);

        Assert.Equal(command.Value, _afterActGoals[1].Value);
        Assert.Equal(command.Type, _afterActGoals[1].Type);
        Assert.Equal(command.PerPeriod, _afterActGoals[1].PerPeriod);
        Assert.Equal(command.StartDate, _afterActGoals[1].StartDate);
        Assert.Equal(command.EndDate ?? DateOnly.MaxValue, _afterActGoals[1].EndDate);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Once(),
            Remove: Times.Never(),
            Update: Times.Once(),
            SaveChanges: Times.Once()
        );
    }

    [Fact]
    public async Task Handle_PartialOverlappingOldGoalAndWithoutForce_ReturnsError()
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 10,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse("02-10-2024"),
            EndDate: null,
            Force: false
        );

        SetExistingGoals(CreateGoal(value: 2000, startDate: "02-01-2024", endDate: "02-15-2024"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        Assert.True(result.IsFailure);

        Assert.Single(_afterActGoals);
        Assert.Equal(_beforeActGoals[0], _afterActGoals[0]);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Never(),
            SaveChanges: Times.Never()
        );
    }

    [Theory]
    [InlineData("02-01-2024", "02-29-2024", "02-01-2024", "02-29-2024")]
    [InlineData("02-01-2024", "02-29-2024", "02-01-2024", "02-05-2024")]
    [InlineData("02-01-2024", "02-29-2024", "02-15-2024", "02-29-2024")]
    [InlineData("02-01-2024", "02-29-2024", "02-01-2024", "02-01-2024")]
    [InlineData("02-01-2024", "02-29-2024", "02-29-2024", "02-29-2024")]
    [InlineData("02-01-2024", null, "02-01-2024", "02-29-2024")]
    [InlineData("02-01-2024", null, "02-15-2024", null)]
    public async Task Handle_FullyOverlappingOldGoalAndWithForce_RemoveOverlappingOldGoalAndCreatesNewGoal(
        string newGoalStartDate,
        string? newGoalEndDate,
        string oldGoalStartDate,
        string? oldGoalEndDate
    )
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 3000,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse(newGoalStartDate),
            EndDate: newGoalEndDate is null ? null : DateOnly.Parse(newGoalEndDate),
            Force: true
        );

        SetExistingGoals(
            CreateGoal(value: 2000, startDate: oldGoalStartDate, endDate: oldGoalEndDate)
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        Assert.True(result.IsSuccess);
        Assert.Single(_afterActGoals);

        Assert.Equal(command.Value, _afterActGoals[0].Value);
        Assert.Equal(command.Type, _afterActGoals[0].Type);
        Assert.Equal(command.PerPeriod, _afterActGoals[0].PerPeriod);
        Assert.Equal(command.StartDate, _afterActGoals[0].StartDate);
        Assert.Equal(command.EndDate ?? DateOnly.MaxValue, _afterActGoals[0].EndDate);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Once(),
            Remove: Times.Once(),
            Update: Times.Never(),
            SaveChanges: Times.Once()
        );
    }

    [Fact]
    public async Task Handle_FullyOverlappingWithOldGoalAndWithoutForce_ReturnsError()
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 10,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse("02-01-2024"),
            EndDate: null,
            Force: false
        );

        SetExistingGoals(CreateGoal(value: 2000, startDate: "02-15-2024", endDate: "02-29-2024"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        Assert.True(result.IsFailure);

        Assert.Single(_afterActGoals);
        Assert.Equal(_beforeActGoals[0], _afterActGoals[0]);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Never(),
            SaveChanges: Times.Never()
        );
    }

    [Fact]
    public async Task Handle_OverlappingMultipleGoalsWithoutForce_ReturnsError()
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 10,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse("02-01-2024"),
            EndDate: null,
            Force: false
        );

        SetExistingGoals(
            CreateGoal(value: 2000, startDate: "02-07-2024", endDate: "02-14-2024"),
            CreateGoal(value: 2000, startDate: "02-15-2024", endDate: "02-29-2024")
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        Assert.True(result.IsFailure);

        Assert.Equal(2, _afterActGoals.Count);

        Assert.Equal(_beforeActGoals[0], _afterActGoals[0]);
        Assert.Equal(_beforeActGoals[1], _afterActGoals[1]);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Never(),
            SaveChanges: Times.Never()
        );
    }

    [Theory]
    [InlineData("02-02-2024", "02-28-2024")]
    public async Task Handle_OverlappingMultipleGoalsWithoutForce_UpdatesFirstAndLastOverlappingGoalsAndRemovesTheOtherOverlappingGoalsAndCreatesTheNewGoal(
        string newGoalStartDate,
        string newGoalEndDate
    )
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 10,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse(newGoalStartDate),
            EndDate: DateOnly.Parse(newGoalEndDate),
            Force: true
        );

        SetExistingGoals(
            CreateGoal(value: 2000, startDate: "02-01-2024", endDate: "02-9-2024"),
            CreateGoal(value: 3000, startDate: "02-10-2024", endDate: "02-12-2024"),
            CreateGoal(value: 3000, startDate: "02-13-2024", endDate: "02-14-2024"),
            CreateGoal(value: 4000, startDate: "02-15-2024", endDate: "02-29-2024")
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        _afterActGoals = _afterActGoals.OrderBy(g => g.StartDate).ToList();

        Assert.True(result.IsSuccess);

        Assert.Equal(3, _afterActGoals.Count);

        Assert.Equal(_beforeActGoals[0].Value, _afterActGoals[0].Value);
        Assert.Equal(_beforeActGoals[0].StartDate, _afterActGoals[0].StartDate);
        Assert.Equal(DateOnly.Parse(newGoalStartDate).AddDays(-1), _afterActGoals[0].EndDate);

        Assert.Equal(command.Value, _afterActGoals[1].Value);
        Assert.Equal(command.Type, _afterActGoals[1].Type);
        Assert.Equal(command.PerPeriod, _afterActGoals[1].PerPeriod);
        Assert.Equal(command.StartDate, _afterActGoals[1].StartDate);
        Assert.Equal(command.EndDate, _afterActGoals[1].EndDate);

        Assert.Equal(_beforeActGoals[3].Value, _afterActGoals[2].Value);
        Assert.Equal(DateOnly.Parse(newGoalEndDate).AddDays(1), _afterActGoals[2].StartDate);
        Assert.Equal(_beforeActGoals[3].EndDate, _afterActGoals[2].EndDate);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Once(),
            Remove: Times.Exactly(2),
            Update: Times.Exactly(2),
            SaveChanges: Times.Once()
        );
    }

    [Fact]
    public async Task Handle_FullyOverlappingMultipleGoalsWithForce_UpdatesLastOverlappingGoalRemoveTheOtherOverlappingGoalsAndCreatesTheNewGoal()
    {
        // Arrange
        var command = new AddUserGoalCommand(
            Value: 10,
            Type: UserGoalType.Calories,
            PerPeriod: UserGoalPerPeriod.Day,
            StartDate: DateOnly.Parse("02-01-2024"),
            EndDate: DateOnly.Parse("02-17-2024"),
            Force: true
        );

        SetExistingGoals(
            CreateGoal(value: 2000, startDate: "02-01-2024", endDate: "02-9-2024"),
            CreateGoal(value: 2000, startDate: "02-10-2024", endDate: "02-14-2024"),
            CreateGoal(value: 2000, startDate: "02-14-2024", endDate: "02-15-2024"),
            CreateGoal(value: 2000, startDate: "02-15-2024", endDate: "02-29-2024")
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert

        Assert.True(result.IsSuccess);

        Assert.Equal(2, _afterActGoals.Count);

        Assert.Equal(command.Value, _afterActGoals[1].Value);
        Assert.Equal(command.Type, _afterActGoals[1].Type);
        Assert.Equal(command.PerPeriod, _afterActGoals[1].PerPeriod);
        Assert.Equal(command.StartDate, _afterActGoals[1].StartDate);
        Assert.Equal(command.EndDate, _afterActGoals[1].EndDate);

        Assert.Equal(DateOnly.Parse("02-18-2024"), _afterActGoals[0].StartDate);

        VerifyCalls(
            GetContainingGoals: Times.Once(),
            Add: Times.Once(),
            Remove: Times.Exactly(3),
            Update: Times.Once(),
            SaveChanges: Times.Once()
        );
    }
}
