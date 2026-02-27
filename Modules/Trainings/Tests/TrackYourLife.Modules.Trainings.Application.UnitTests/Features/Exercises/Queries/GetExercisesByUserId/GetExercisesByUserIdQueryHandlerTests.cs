using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExercisesByUserId;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Queries.GetExercisesByUserId;

public class GetExercisesByUserIdQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesQuery _exerciseQuery;
    private readonly GetExercisesByUserIdQueryHandler _handler;

    private readonly UserId _userId;

    public GetExercisesByUserIdQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exerciseQuery = Substitute.For<IExercisesQuery>();
        _handler = new GetExercisesByUserIdQueryHandler(_userIdentifierProvider, _exerciseQuery);

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoExercisesFound_ShouldReturnEmptyCollection()
    {
        // Arrange
        _exerciseQuery
            .GetEnumerableByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetExercisesByUserIdQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenExercisesFound_ShouldReturnExercises()
    {
        // Arrange
        var exercises = new List<ExerciseReadModel>
        {
            ExerciseReadModelFaker.Generate(),
            ExerciseReadModelFaker.Generate(),
        };

        _exerciseQuery
            .GetEnumerableByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var query = new GetExercisesByUserIdQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(exercises);
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        // Arrange
        var exercises = new List<ExerciseReadModel> { ExerciseReadModelFaker.Generate() };

        _exerciseQuery
            .GetEnumerableByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(exercises);

        var query = new GetExercisesByUserIdQuery();

        // Act
        await _handler.Handle(query, default);

        // Assert
        await _exerciseQuery
            .Received(1)
            .GetEnumerableByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }
}

