using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.CreateTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Commands.CreateTraining;

public class CreateTrainingCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ITrainingsRepository _trainingsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreateTrainingCommandHandler _handler;

    private readonly UserId _userId;
    private readonly string _name;
    private readonly List<string> _muscleGroups;
    private readonly Difficulty _difficulty;
    private readonly List<ExerciseId> _exercisesIds;
    private readonly int _duration;
    private readonly int _restSeconds;
    private readonly string? _description;

    public CreateTrainingCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exercisesRepository = Substitute.For<IExercisesRepository>();
        _trainingsRepository = Substitute.For<ITrainingsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new CreateTrainingCommandHandler(
            _userIdentifierProvider,
            _exercisesRepository,
            _trainingsRepository,
            _dateTimeProvider
        );

        _userId = UserId.NewId();
        _name = "Test Training";
        _muscleGroups = ["Chest", "Triceps"];
        _difficulty = Difficulty.Easy;
        _exercisesIds = [ExerciseId.NewId(), ExerciseId.NewId()];
        _duration = 60;
        _restSeconds = 90;
        _description = "Test description";

        _userIdentifierProvider.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenSomeExercisesNotFound_ShouldReturnFailure()
    {
        // Arrange
        var exercises = new List<Exercise> { ExerciseFaker.Generate(id: _exercisesIds[0]) };

        _exercisesRepository
            .GetEnumerableWithinIdsCollectionAsync(_exercisesIds, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var command = new CreateTrainingCommand(
            _name,
            _muscleGroups,
            _difficulty,
            _exercisesIds,
            _duration,
            _restSeconds,
            _description
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.NotFoundById(_exercisesIds[1]));
        await _trainingsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Training>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTrainingExerciseCreationFails_ShouldReturnFailure()
    {
        // Arrange
        // Return fewer exercises than requested to simulate missing exercises
        var exercises = new List<Exercise> { ExerciseFaker.Generate(id: _exercisesIds[0]) };

        _exercisesRepository
            .GetEnumerableWithinIdsCollectionAsync(_exercisesIds, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var command = new CreateTrainingCommand(
            _name,
            _muscleGroups,
            _difficulty,
            _exercisesIds,
            _duration,
            _restSeconds,
            _description
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        await _trainingsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Training>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTrainingCreationFails_ShouldReturnFailure()
    {
        // Arrange
        var exercises = _exercisesIds.Select(id => ExerciseFaker.Generate(id: id)).ToList();

        _exercisesRepository
            .GetEnumerableWithinIdsCollectionAsync(_exercisesIds, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var command = new CreateTrainingCommand(
            "", // Invalid name to force creation failure
            _muscleGroups,
            _difficulty,
            _exercisesIds,
            _duration,
            _restSeconds,
            _description
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        await _trainingsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Training>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldCreateTraining()
    {
        // Arrange
        var exercises = _exercisesIds.Select(id => ExerciseFaker.Generate(id: id)).ToList();

        _exercisesRepository
            .GetEnumerableWithinIdsCollectionAsync(_exercisesIds, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var command = new CreateTrainingCommand(
            _name,
            _muscleGroups,
            _difficulty,
            _exercisesIds,
            _duration,
            _restSeconds,
            _description
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _trainingsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Training>(t =>
                    t.UserId == _userId
                    && t.Name == _name
                    && t.MuscleGroups.SequenceEqual(_muscleGroups)
                    && t.Difficulty == _difficulty
                    && t.Duration == _duration
                    && t.RestSeconds == _restSeconds
                    && t.Description == _description
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
