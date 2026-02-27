using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Commands.DeleteExerciseHistory;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Commands.DeleteExerciseHistory;

public class DeleteExerciseHistoryCommandHandlerTests
{
    private readonly IExercisesHistoriesRepository _exercisesHistoriesRepository;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteExerciseHistoryCommandHandler _handler;

    private readonly UserId _userId;
    private readonly ExerciseHistoryId _exerciseHistoryId;
    private readonly ExerciseId _exerciseId;

    public DeleteExerciseHistoryCommandHandlerTests()
    {
        _exercisesHistoriesRepository = Substitute.For<IExercisesHistoriesRepository>();
        _exercisesRepository = Substitute.For<IExercisesRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteExerciseHistoryCommandHandler(
            _exercisesHistoriesRepository,
            _exercisesRepository,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _exerciseHistoryId = ExerciseHistoryId.NewId();
        _exerciseId = ExerciseId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenExerciseHistoryNotFound_ShouldReturnFailure()
    {
        // Arrange
        _exercisesHistoriesRepository
            .GetByIdAsync(_exerciseHistoryId, Arg.Any<CancellationToken>())
            .Returns((ExerciseHistory?)null);

        var command = new DeleteExerciseHistoryCommand(_exerciseHistoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExerciseHistoryErrors.NotFoundById(_exerciseHistoryId));
        _exercisesHistoriesRepository.DidNotReceive().Remove(Arg.Any<ExerciseHistory>());
    }

    [Fact]
    public async Task Handle_WhenExerciseNotFound_ShouldReturnFailure()
    {
        // Arrange
        var exerciseHistory = ExerciseHistoryFaker.GenerateEntity(
            id: _exerciseHistoryId,
            exerciseId: _exerciseId
        );

        _exercisesHistoriesRepository
            .GetByIdAsync(_exerciseHistoryId, Arg.Any<CancellationToken>())
            .Returns(exerciseHistory);
        _exercisesRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var command = new DeleteExerciseHistoryCommand(_exerciseHistoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExerciseHistoryErrors.NotOwned(_exerciseHistoryId));
        _exercisesHistoriesRepository.DidNotReceive().Remove(Arg.Any<ExerciseHistory>());
    }

    [Fact]
    public async Task Handle_WhenExerciseBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var differentUserId = UserId.NewId();
        var exerciseHistory = ExerciseHistoryFaker.GenerateEntity(
            id: _exerciseHistoryId,
            exerciseId: _exerciseId
        );
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: differentUserId);

        _exercisesHistoriesRepository
            .GetByIdAsync(_exerciseHistoryId, Arg.Any<CancellationToken>())
            .Returns(exerciseHistory);
        _exercisesRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var command = new DeleteExerciseHistoryCommand(_exerciseHistoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExerciseHistoryErrors.NotOwned(_exerciseHistoryId));
        _exercisesHistoriesRepository.DidNotReceive().Remove(Arg.Any<ExerciseHistory>());
    }

    [Fact]
    public async Task Handle_WhenValid_ShouldRemoveExerciseHistoryAndReturnSuccess()
    {
        // Arrange
        var exerciseHistory = ExerciseHistoryFaker.GenerateEntity(
            id: _exerciseHistoryId,
            exerciseId: _exerciseId
        );
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);

        _exercisesHistoriesRepository
            .GetByIdAsync(_exerciseHistoryId, Arg.Any<CancellationToken>())
            .Returns(exerciseHistory);
        _exercisesRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var command = new DeleteExerciseHistoryCommand(_exerciseHistoryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _exercisesHistoriesRepository.Received(1).Remove(exerciseHistory);
    }
}
