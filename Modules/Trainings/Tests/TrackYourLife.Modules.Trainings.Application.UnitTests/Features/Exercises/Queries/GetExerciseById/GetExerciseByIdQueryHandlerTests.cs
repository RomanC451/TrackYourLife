using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseById;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Queries.GetExerciseById;

public class GetExerciseByIdQueryHandlerTests
{
    private readonly IExercisesQuery _exerciseQuery;
    private readonly ISupaBaseStorage _supaBaseStorage;
    private readonly GetExerciseByIdQueryHandler _handler;

    private readonly ExerciseId _exerciseId;

    public GetExerciseByIdQueryHandlerTests()
    {
        _exerciseQuery = Substitute.For<IExercisesQuery>();
        _supaBaseStorage = Substitute.For<ISupaBaseStorage>();
        _handler = new GetExerciseByIdQueryHandler(_exerciseQuery, _supaBaseStorage);

        _exerciseId = ExerciseId.NewId();
    }

    [Fact]
    public async Task Handle_WhenExerciseNotFound_ShouldReturnFailure()
    {
        // Arrange
        _exerciseQuery
            .GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>())
            .Returns((ExerciseReadModel?)null);

        var query = new GetExerciseByIdQuery(_exerciseId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ExercisesErrors.NotFoundById(_exerciseId));
    }

    [Fact]
    public async Task Handle_WhenExerciseFoundWithoutPicture_ShouldReturnExercise()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(id: _exerciseId, pictureUrl: null);

        _exerciseQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);

        var query = new GetExerciseByIdQuery(_exerciseId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(exercise);
        await _supaBaseStorage
            .DidNotReceive()
            .CreateSignedUrlAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_WhenExerciseFoundWithPictureAndSignedUrlCreationFails_ShouldReturnExerciseWithEmptyPictureUrl()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(
            id: _exerciseId,
            pictureUrl: "test-picture.jpg"
        );

        _exerciseQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);
        _supaBaseStorage
            .CreateSignedUrlAsync("images", "test-picture.jpg")
            .Returns(
                Result.Failure<string>(
                    new Error("SignedUrlCreationFailed", "Signed URL creation failed")
                )
            );

        var query = new GetExerciseByIdQuery(_exerciseId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PictureUrl.Should().Be("");
    }

    [Fact]
    public async Task Handle_WhenExerciseFoundWithPictureAndSignedUrlCreationSucceeds_ShouldReturnExerciseWithSignedUrl()
    {
        // Arrange
        var exercise = ExerciseReadModelFaker.Generate(
            id: _exerciseId,
            pictureUrl: "test-picture.jpg"
        );
        var signedUrl = "https://signed-url.com/test-picture.jpg";

        _exerciseQuery.GetByIdAsync(_exerciseId, Arg.Any<CancellationToken>()).Returns(exercise);
        _supaBaseStorage
            .CreateSignedUrlAsync("images", "test-picture.jpg")
            .Returns(Result.Success(signedUrl));

        var query = new GetExerciseByIdQuery(_exerciseId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PictureUrl.Should().Be(signedUrl);
    }
}
