using System.Collections.ObjectModel;
using Moq;
using TrackYourLife.Application.UserGoals.Commands.UpdateUserGoal;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.UserGoals.Commands.UpdateUserGoal;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.UserGoals;
using TrackYourLife.Common.Domain.Users.StrongTypes;

namespace TrackYourLife.Common.Application.UnitTests.UserGoals;

public sealed class UpdateUserGoalCommandHandlerTests
{
    private readonly Mock<IUserGoalRepository> _userGoalRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IUserIdentifierProvider> _userIdentifierProvider = new();
    private readonly UpdateUserGoalCommandHandler _sut;

    private List<UserGoal> _afterActGoals;

    private ReadOnlyCollection<UserGoal> _beforeActGoals;

    public UpdateUserGoalCommandHandlerTests()
    {
        _sut = new UpdateUserGoalCommandHandler(
            _userGoalRepository.Object,
            _unitOfWork.Object,
            _userIdentifierProvider.Object
        );

        _afterActGoals = new();
        _beforeActGoals = new([]);

        SetupDependencies();
    }

    private void SetupDependencies()
    {
        _userGoalRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<UserGoalId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (UserGoalId id, CancellationToken _) =>
                {
                    return _afterActGoals.FirstOrDefault(g => g.Id == id);
                }
            );

        _userGoalRepository
            .Setup(
                repo =>
                    repo.GetOverlappingGoalsAsync(
                        It.IsAny<UserGoal>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(_afterActGoals.ToList());

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
    }

    private void SetExistingGoals(params UserGoal[] goals)
    {
        _afterActGoals.Clear();
        _afterActGoals.AddRange(goals
                .Select(
                    goal =>
                        UserGoal.Create(
                            id: goal.Id,
                            userId: goal.UserId,
                            type: goal.Type,
                            value: goal.Value,
                            perPeriod: goal.PerPeriod,
                            startDate: goal.StartDate,
                            endDate: goal.EndDate
                        ).Value
                )
                .ToList());

        _beforeActGoals = new ReadOnlyCollection<UserGoal>(
            goals
                .Select(
                    goal =>
                        UserGoal.Create(
                            id: goal.Id,
                            userId: goal.UserId,
                            type: goal.Type,
                            value: goal.Value,
                            perPeriod: goal.PerPeriod,
                            startDate: goal.StartDate,
                            endDate: goal.EndDate
                        ).Value
                )
                .ToList()
        );

        SetupDependencies();
    }

    private static UserGoal CreateGoal(
        int value,
        string startDate,
        string? endDate,
        UserGoalId? id = null,
        UserId? userId = null
    ) =>
        UserGoal.Create(
            id: id ?? UserGoalId.NewId(),
            userId: userId ?? UserId.NewId(),
            type: UserGoalType.Calories,
            value: value,
            perPeriod: UserGoalPerPeriod.Day,
            startDate: DateOnly.Parse(startDate),
            endDate: !string.IsNullOrEmpty(endDate) ? DateOnly.Parse(endDate) : null
        ).Value;

    private void VerifyCalls(
        Times GetById,
        Times GetContainingGoals,
        Times Add,
        Times Remove,
        Times Update,
        Times SaveChanges
    )
    {
        // Verify that the repository methods were called correctly

        _userGoalRepository.Verify(
            repo => repo.GetByIdAsync(It.IsAny<UserGoalId>(), It.IsAny<CancellationToken>()),
            GetById
        );

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
    public async Task Handle_WhenUserGoalNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Calories,
            Value: 300,
            PerPeriod: UserGoalPerPeriod.Month,
            StartDate: DateOnly.Parse("01-01-2024"),
            EndDate: DateOnly.Parse("01-31-2024")
        );

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.UserGoal.NotFound(command.Id).Code, result.Error.Code);

        VerifyCalls(
            GetById: Times.Once(),
            GetContainingGoals: Times.Never(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Never(),
            SaveChanges: Times.Never()
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalNotOwned_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Calories,
            Value: 300,
            PerPeriod: UserGoalPerPeriod.Month,
            StartDate: DateOnly.Parse("01-01-2024"),
            EndDate: DateOnly.Parse("01-31-2024")
        );

        var userGoal = CreateGoal(
            value: 300,
            startDate: "01-01-2024",
            endDate: "01-31-2024",
            id: command.Id
        );
        SetExistingGoals(userGoal);

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(UserId.NewId());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.UserGoal.NotOwned(command.Id).Code, result.Error.Code);

        VerifyCalls(
            GetById: Times.Once(),
            GetContainingGoals: Times.Never(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Never(),
            SaveChanges: Times.Never()
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalFoundAndOwned_ShouldUpdateUserGoal()
    {
        // Arrange
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Calories,
            Value: 300,
            PerPeriod: UserGoalPerPeriod.Month,
            StartDate: DateOnly.Parse("01-01-2024"),
            EndDate: DateOnly.Parse("01-31-2024")
        );
        var userId = UserId.NewId();

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(userId);

        var userGoal = CreateGoal(
            value: 300,
            startDate: "01-01-2024",
            endDate: "01-31-2024",
            id: command.Id,
            userId: userId
        );
        SetExistingGoals(userGoal);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        VerifyCalls(
            GetById: Times.Once(),
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Once(),
            SaveChanges: Times.Once()
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalFoundAndOwnedAndOverlapping_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Calories,
            Value: 300,
            PerPeriod: UserGoalPerPeriod.Month,
            StartDate: DateOnly.Parse("01-01-2024"),
            EndDate: DateOnly.Parse("01-31-2024")
        );

        var userId = UserId.NewId();

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(userId);

        var userGoal = CreateGoal(
            value: 300,
            startDate: "01-01-2024",
            endDate: "01-31-2024",
            id: command.Id,
            userId: userId
        );
        var overlappingGoal = CreateGoal(300, "01-15-2024", "02-15-2024");
        SetExistingGoals(userGoal, overlappingGoal);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.UserGoal.Overlapping(command.Type).Code, result.Error.Code);

        VerifyCalls(
            GetById: Times.Once(),
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Never(),
            SaveChanges: Times.Never()
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalFoundAndOwnedAndOverlappingAndForce_ShouldUpdateUserGoal()
    {
        // Arrange
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Water,
            Value: 300,
            PerPeriod: UserGoalPerPeriod.Month,
            StartDate: DateOnly.Parse("01-01-2024"),
            EndDate: DateOnly.Parse("01-31-2024"),
            Force: true
        );

        var userId = UserId.NewId();

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(userId);

        var userGoal = CreateGoal(
            value: 300,
            startDate: "01-01-2024",
            endDate: "01-15-2024",
            id: command.Id,
            userId: userId
        );
        var overlappingGoal = CreateGoal(300, "01-15-2024", "02-15-2024");
        SetExistingGoals(userGoal, overlappingGoal);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, _afterActGoals.Count);

        Assert.Equal(command.Id, _afterActGoals[0].Id);
        Assert.Equal(command.Type, _afterActGoals[0].Type);
        Assert.Equal(command.Value, _afterActGoals[0].Value);
        Assert.Equal(command.PerPeriod, _afterActGoals[0].PerPeriod);
        Assert.Equal(command.StartDate, _afterActGoals[0].StartDate);
        Assert.Equal(command.EndDate, _afterActGoals[0].EndDate);


        Assert.Equal(overlappingGoal.Id, _afterActGoals[1].Id);
        Assert.Equal(overlappingGoal.Type, _afterActGoals[1].Type);
        Assert.Equal(overlappingGoal.Value, _afterActGoals[1].Value);
        Assert.Equal(overlappingGoal.PerPeriod, _afterActGoals[1].PerPeriod);
        Assert.Equal(command.EndDate!.Value.AddDays(1), _afterActGoals[1].StartDate);
        Assert.Equal(overlappingGoal.EndDate, _afterActGoals[1].EndDate);

        VerifyCalls(
            GetById: Times.Once(),
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.Never(),
            Update: Times.Exactly(2),
            SaveChanges: Times.Once()
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalFoundAndOwnedAndOverlappingAndForceAndFullyOverlapped_ShouldUpdateUserGoal()
    {
        // Arrange
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Water,
            Value: 300,
            PerPeriod: UserGoalPerPeriod.Month,
            StartDate: DateOnly.Parse("01-01-2024"),
            EndDate: DateOnly.Parse("01-31-2024"),
            Force: true
        );

        var userId = UserId.NewId();

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(userId);

        var userGoal = CreateGoal(
            value: 300,
            startDate: "01-01-2024",
            endDate: "01-15-2024",
            id: command.Id,
            userId: userId
        );
        var overlappingGoal = CreateGoal(300, "01-15-2024", "01-31-2024");
        SetExistingGoals(userGoal, overlappingGoal);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(_afterActGoals);

        Assert.Equal(command.Id, _afterActGoals[0].Id);
        Assert.Equal(command.Type, _afterActGoals[0].Type);
        Assert.Equal(command.Value, _afterActGoals[0].Value);
        Assert.Equal(command.PerPeriod, _afterActGoals[0].PerPeriod);
        Assert.Equal(command.StartDate, _afterActGoals[0].StartDate);
        Assert.Equal(command.EndDate, _afterActGoals[0].EndDate);

        VerifyCalls(
            GetById: Times.Once(),
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.Once(),
            Update: Times.Once(),
            SaveChanges: Times.Once()
        );
    }

    [Fact]
    public async Task Handle_WhenUserGoalFoundAndOwnedAndOverlappingAndForceAndFullyOverlappedAndMultipleGoals_ShouldUpdateUserGoal()
    {
        // Arrange
        var command = new UpdateUserGoalCommand(
            Id: UserGoalId.NewId(),
            Type: UserGoalType.Water,
            Value: 300,
            PerPeriod: UserGoalPerPeriod.Month,
            StartDate: DateOnly.Parse("01-01-2024"),
            EndDate: DateOnly.Parse("01-31-2024"),
            Force: true
        );

        var userId = UserId.NewId();

        _userIdentifierProvider.SetupGet(provider => provider.UserId).Returns(userId);

        var userGoal = CreateGoal(
            value: 300,
            startDate: "01-01-2024",
            endDate: "01-15-2024",
            id: command.Id,
            userId: userId
        );
        var overlappingGoal1 = CreateGoal(300, "12-01-2023", "01-31-2024");
        var overlappingGoal2 = CreateGoal(300, "02-01-2024", "02-15-2024");
        SetExistingGoals(userGoal, overlappingGoal1, overlappingGoal2);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);

        var updatedGoal = _afterActGoals.FirstOrDefault(g => g.Id == command.Id);

        Assert.NotNull(updatedGoal);

        Assert.Equal(command.Id, updatedGoal.Id);
        Assert.Equal(command.Type, updatedGoal.Type);
        Assert.Equal(command.Value, updatedGoal.Value);
        Assert.Equal(command.PerPeriod, updatedGoal.PerPeriod);
        Assert.Equal(command.StartDate, updatedGoal.StartDate);
        Assert.Equal(command.EndDate, updatedGoal.EndDate);


        VerifyCalls(
            GetById: Times.Once(),
            GetContainingGoals: Times.Once(),
            Add: Times.Never(),
            Remove: Times.AtMost(2),
            Update: Times.AtMost(3),
            SaveChanges: Times.Once()
        );

    }

}
