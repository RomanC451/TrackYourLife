using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Commands.CreateExercise;

public class CreateExerciseCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesRepository _exercisesRepository;
    private readonly ISupaBaseStorage _supaBaseStorage;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreateExerciseCommandHandler _handler;

    private readonly UserId _userId;
    private readonly string _name;
    private readonly List<string> _muscleGroups;
    private readonly Difficulty _difficulty;
    private readonly string? _pictureUrl;
    private readonly string? _videoUrl;
    private readonly string? _description;
    private readonly string? _equipment;
    private readonly List<ExerciseSet> _sets;

    public CreateExerciseCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exercisesRepository = Substitute.For<IExercisesRepository>();
        _supaBaseStorage = Substitute.For<ISupaBaseStorage>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _handler = new CreateExerciseCommandHandler(
            _userIdentifierProvider,
            _exercisesRepository,
            _supaBaseStorage,
            _dateTimeProvider
        );

        _userId = UserId.NewId();
        _name = "Test Exercise";
        _muscleGroups = ["Chest", "Triceps"];
        _difficulty = Difficulty.Easy;
        _pictureUrl = "test-picture-url";
        _videoUrl = "test-video-url";
        _description = "Test description";
        _equipment = "Barbell";
        _sets = [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 60, "kg").Value];

        _userIdentifierProvider.UserId.Returns(_userId);
        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_WhenImageProcessingFails_ShouldReturnFailure()
    {
        // Arrange
        _supaBaseStorage
            .RenameFileFromSignedUrlAsync(_pictureUrl!, Arg.Any<string>())
            .Returns(
                Result.Failure<string>(
                    new Error("ImageProcessingFailed", "Image processing failed")
                )
            );

        var command = new CreateExerciseCommand(
            _name,
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _sets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Image processing failed");
        await _exercisesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Exercise>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenExerciseCreationFails_ShouldReturnFailure()
    {
        // Arrange
        _supaBaseStorage
            .RenameFileFromSignedUrlAsync(_pictureUrl!, Arg.Any<string>())
            .Returns(Result.Success("new-image-url"));

        var command = new CreateExerciseCommand(
            "", // Invalid name to force creation failure
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _sets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        await _exercisesRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Exercise>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldCreateExercise()
    {
        // Arrange
        _supaBaseStorage
            .RenameFileFromSignedUrlAsync(_pictureUrl!, Arg.Any<string>())
            .Returns(Result.Success("new-image-url"));

        var command = new CreateExerciseCommand(
            _name,
            _muscleGroups,
            _difficulty,
            _pictureUrl,
            _videoUrl,
            _description,
            _equipment,
            _sets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _exercisesRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Exercise>(e =>
                    e.UserId == _userId
                    && e.Name == _name
                    && e.MuscleGroups.SequenceEqual(_muscleGroups)
                    && e.Difficulty == _difficulty
                    && e.PictureUrl == "new-image-url"
                    && e.VideoUrl == _videoUrl
                    && e.Description == _description
                    && e.Equipment == _equipment
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenNoPictureUrl_ShouldCreateExerciseWithoutImageProcessing()
    {
        // Arrange
        var command = new CreateExerciseCommand(
            _name,
            _muscleGroups,
            _difficulty,
            null, // No picture URL
            _videoUrl,
            _description,
            _equipment,
            _sets
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _supaBaseStorage
            .DidNotReceive()
            .RenameFileFromSignedUrlAsync(Arg.Any<string>(), Arg.Any<string>());
        await _exercisesRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Exercise>(e =>
                    e.UserId == _userId && e.Name == _name && e.PictureUrl == string.Empty
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
