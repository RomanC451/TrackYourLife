using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

public class GetOngoingTrainingByIdQueryHandlerTests
{
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly IExercisesHistoriesQuery _exercisesHistoriesQuery;
    private readonly ISupaBaseStorage _supaBaseStorage;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetOngoingTrainingByIdQueryHandler _handler;

    private readonly UserId _userId;
    private readonly OngoingTrainingId _ongoingTrainingId;

    public GetOngoingTrainingByIdQueryHandlerTests()
    {
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _exercisesHistoriesQuery = Substitute.For<IExercisesHistoriesQuery>();
        _supaBaseStorage = Substitute.For<ISupaBaseStorage>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetOngoingTrainingByIdQueryHandler(
            _ongoingTrainingsQuery,
            _exercisesHistoriesQuery,
            _supaBaseStorage,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _ongoingTrainingId = OngoingTrainingId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingNotFound_ShouldReturnFailure()
    {
        // Arrange
        _ongoingTrainingsQuery
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns((OngoingTrainingReadModel?)null);

        var query = new GetOngoingTrainingByIdQuery(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OngoingTrainingErrors.NotFoundById(_ongoingTrainingId));
    }

    [Fact]
    public async Task Handle_WhenOngoingTrainingFound_ShouldReturnOngoingTraining()
    {
        // Arrange
        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(
            id: _ongoingTrainingId,
            userId: _userId
        );
        var exerciseHistories = new List<ExerciseHistoryReadModel>
        {
            ExerciseHistoryReadModelFaker.Generate(ongoingTrainingId: _ongoingTrainingId),
            ExerciseHistoryReadModelFaker.Generate(ongoingTrainingId: _ongoingTrainingId),
        };

        _ongoingTrainingsQuery
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        _exercisesHistoriesQuery
            .GetByOngoingTrainingIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(exerciseHistories);

        var query = new GetOngoingTrainingByIdQuery(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OngoingTraining.Should().Be(ongoingTraining);
        result.Value.ExerciseHistories.Should().BeEquivalentTo(exerciseHistories);
    }
}
