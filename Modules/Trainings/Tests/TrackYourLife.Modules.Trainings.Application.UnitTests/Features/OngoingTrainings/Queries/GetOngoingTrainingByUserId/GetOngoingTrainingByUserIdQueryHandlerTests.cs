using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

public class GetOngoingTrainingByUserIdQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly ISupaBaseStorage _supaBaseStorage;
    private readonly GetOngoingTrainingByUserIdQueryHandler _handler;

    private readonly UserId _userId;

    public GetOngoingTrainingByUserIdQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _supaBaseStorage = Substitute.For<ISupaBaseStorage>();
        _handler = new GetOngoingTrainingByUserIdQueryHandler(
            _ongoingTrainingsQuery,
            _exercisesHistoriesQuery,
            _userIdentifierProvider,
            _supaBaseStorage
        );

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoOngoingTrainingFound_ShouldReturnNotFoundError()
    {
        // Arrange
        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var query = new GetOngoingTrainingByUserIdQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingFound_ShouldReturnOngoingTraining()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(userId: _userId);
        var exerciseHistories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(ongoingTrainingId: ongoingTraining.Id),
            ExerciseHistoryReadModelFaker.Generate(ongoingTrainingId: ongoingTraining.Id),
        };

        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        _exercisesHistoriesQuery
            .GetByOngoingTrainingIdAsync(ongoingTraining.Id, Arg.Any<CancellationToken>())
            .Returns(exerciseHistories);

        var query = new GetOngoingTrainingByUserIdQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OngoingTraining.Should().Be(ongoingTraining);
        result.Value.ExerciseHistories.Should().BeEquivalentTo(exerciseHistories);
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(userId: _userId);
        var exerciseHistories = new List<ExerciseHistoryReadModel>();

        _ongoingTrainingsQuery
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        _exercisesHistoriesQuery
            .GetByOngoingTrainingIdAsync(ongoingTraining.Id, Arg.Any<CancellationToken>())
            .Returns(exerciseHistories);

        var query = new GetOngoingTrainingByUserIdQuery();

        // Act
        await _handler.Handle(query, default);

        // Assert
        await _ongoingTrainingsQuery
            .Received(1)
            .GetUnfinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>());
        await _exercisesHistoriesQuery
            .Received(1)
            .GetByOngoingTrainingIdAsync(ongoingTraining.Id, Arg.Any<CancellationToken>());
    }
}
