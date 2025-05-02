using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Commands.CalculateNutritionGoals;

public class CalculateNutritionGoalsCommandHandlerTests
{
    private readonly IGoalRepository _goalRepository;
    private readonly IGoalsManagerService _goalsManagerService;
    private readonly INutritionGoalsCalculator _nutritionCalculator;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly CalculateNutritionGoalsCommandHandler _handler;

    public CalculateNutritionGoalsCommandHandlerTests()
    {
        _goalRepository = Substitute.For<IGoalRepository>();
        _goalsManagerService = Substitute.For<IGoalsManagerService>();
        _nutritionCalculator = Substitute.For<INutritionGoalsCalculator>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new CalculateNutritionGoalsCommandHandler(
            _goalRepository,
            _goalsManagerService,
            _nutritionCalculator,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WithValidInput_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _nutritionCalculator
            .CalculateCalories(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Gender>(),
                Arg.Any<ActivityLevel>(),
                Arg.Any<FitnessGoal>()
            )
            .Returns(2000);
        _nutritionCalculator.CalculateProtein(Arg.Any<int>()).Returns(112);
        _nutritionCalculator.CalculateCarbs(Arg.Any<int>()).Returns(250);
        _nutritionCalculator.CalculateFat(Arg.Any<int>()).Returns(67);

        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success(false));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _goalRepository.Received(4).AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOverlappingGoalsAndForceFalse_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _nutritionCalculator
            .CalculateCalories(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Gender>(),
                Arg.Any<ActivityLevel>(),
                Arg.Any<FitnessGoal>()
            )
            .Returns(2000);
        _nutritionCalculator.CalculateProtein(Arg.Any<int>()).Returns(112);
        _nutritionCalculator.CalculateCarbs(Arg.Any<int>()).Returns(250);
        _nutritionCalculator.CalculateFat(Arg.Any<int>()).Returns(67);

        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Failure<bool>(GoalErrors.Overlapping(GoalType.Calories)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.Overlapping(GoalType.Calories));
        await _goalRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithOverlappingGoalsAndForceTrue_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: true
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _nutritionCalculator
            .CalculateCalories(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Gender>(),
                Arg.Any<ActivityLevel>(),
                Arg.Any<FitnessGoal>()
            )
            .Returns(2000);
        _nutritionCalculator.CalculateProtein(Arg.Any<int>()).Returns(112);
        _nutritionCalculator.CalculateCarbs(Arg.Any<int>()).Returns(250);
        _nutritionCalculator.CalculateFat(Arg.Any<int>()).Returns(67);

        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success(false));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _goalRepository.Received(4).AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidGoalCreation_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var command = new CalculateNutritionGoalsCommand(
            Age: 25,
            Weight: 70,
            Height: 180,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _nutritionCalculator
            .CalculateCalories(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Gender>(),
                Arg.Any<ActivityLevel>(),
                Arg.Any<FitnessGoal>()
            )
            .Returns(-1); // Invalid value that will cause goal creation to fail
        _goalsManagerService
            .HandleOverlappingGoalsAsync(
                Arg.Any<Goal>(),
                command.Force,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success(false));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        await _goalRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Goal>(), Arg.Any<CancellationToken>());
    }
}
