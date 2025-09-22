using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Commands.UpdateExercise;

public class UpdateExerciseCommandHandlerTests
{
    private readonly IExercisesRepository _exerciseRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ISupaBaseStorage _supaBaseStorage;
    private readonly UpdateExerciseCommandHandler _handler;

    private readonly UserId _userId;
    private readonly ExerciseId _exerciseId;
    private readonly string _name;
    private readonly List<string> _muscleGroups;
    private readonly Difficulty _difficulty;
    private readonly string? _pictureUrl;
    private readonly string? _videoUrl;
    private readonly string? _description;
    private readonly string? _equipment;
    private readonly List<ExerciseSet> _exerciseSets;

    public UpdateExerciseCommandHandlerTests()
    {
        _exerciseRepository = Substitute.For<IExercisesRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _supaBaseStorage = Substitute.For<ISupaBaseStorage>();
        _handler = new UpdateExerciseCommandHandler(
            _exerciseRepository,
            _userIdentifierProvider,
            _supaBaseStorage
        );

        _userId = UserId.NewId();
        _exerciseId = ExerciseId.NewId();
        _name = "Updated Exercise";
        _muscleGroups = ["Chest", "Triceps"];
        _difficulty = Difficulty.Medium;
        _pictureUrl = "test-picture-url";
        _videoUrl = "test-video-url";
        _description = "Updated description";
        _equipment = "Dumbbells";
        _exerciseSets = [new ExerciseSet(Guid.NewGuid(), "Set 1", 12, 90, 0)];

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    private void ClearMocks()
    {
        _exerciseRepository.ClearSubstitute();
        _userIdentifierProvider.ClearSubstitute();
        _supaBaseStorage.ClearSubstitute();

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenExerciseNotFound_ShouldReturnFailure()
    {
        // Arrange
        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var command = new UpdateExerciseCommand(
            _exerciseId,
            _name,
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _exerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.NotFoundById(_exerciseId));
        _exerciseRepository.DidNotReceive().Update(Arg.Any<Exercise>());
    }

    [Fact]
    public async Task Handle_WhenExerciseBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var differentUserId = UserId.NewId();
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: differentUserId);

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var command = new UpdateExerciseCommand(
            _exerciseId,
            _name,
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _exerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.NotFoundById(_exerciseId));
        _exerciseRepository.DidNotReceive().Update(Arg.Any<Exercise>());
    }

    [Fact]
    public async Task Handle_WhenImageProcessingFails_ShouldReturnFailure()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);
        _supaBaseStorage
            .RenameFileFromSignedUrlAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(
                Task.FromResult(
                    Result.Failure<string>(
                        new Error("ImageProcessingFailed", "Image processing failed")
                    )
                )
            );

        var command = new UpdateExerciseCommand(
            _exerciseId,
            _name,
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _exerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Image processing failed");
        _exerciseRepository.DidNotReceive().Update(Arg.Any<Exercise>());
    }

    [Fact]
    public async Task Handle_WhenExerciseUpdateFails_ShouldReturnFailure()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);
        _supaBaseStorage
            .RenameFileFromSignedUrlAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(Result.Success("new-image-url")));

        var command = new UpdateExerciseCommand(
            _exerciseId,
            "", // Invalid name to force update failure
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _exerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        _exerciseRepository.DidNotReceive().Update(Arg.Any<Exercise>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldUpdateExercise()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);
        _supaBaseStorage
            .RenameFileFromSignedUrlAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(Result.Success("new-image-url")));

        var command = new UpdateExerciseCommand(
            _exerciseId,
            _name,
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _exerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _exerciseRepository.Received(1).Update(exercise);
    }

    [Fact]
    public async Task Handle_WhenNoPictureUrl_ShouldUpdateExerciseWithoutImageProcessing()
    {
        // Arrange
        ClearMocks();
        var exercise = ExerciseFaker.Generate(id: _exerciseId, userId: _userId);

        _exerciseRepository
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns(exercise);

        // Ensure the mock is not set up for this test
        _supaBaseStorage
            .RenameFileFromSignedUrlAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult(Result.Success("test-url")));

        var command = new UpdateExerciseCommand(
            _exerciseId,
            _name,
            _muscleGroups,
            _difficulty,
            _description,
            _videoUrl,
            null, // No picture URL
            _equipment,
            _exerciseSets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _supaBaseStorage
            .DidNotReceive()
            .RenameFileFromSignedUrlAsync(Arg.Any<string>(), Arg.Any<string>());
        _exerciseRepository.Received(1).Update(exercise);
    }
}
