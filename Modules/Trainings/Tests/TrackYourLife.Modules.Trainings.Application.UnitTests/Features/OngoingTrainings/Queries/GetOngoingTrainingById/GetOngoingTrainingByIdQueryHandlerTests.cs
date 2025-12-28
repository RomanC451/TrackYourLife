using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

public class GetOngoingTrainingByIdQueryHandlerTests
{
    private readonly IOngoingTrainingsQuery _ongoingTrainingsQuery;
    private readonly ISupaBaseStorage _supaBaseStorage;
    private readonly GetOngoingTrainingByIdQueryHandler _handler;

    private readonly OngoingTrainingId _ongoingTrainingId;

    public GetOngoingTrainingByIdQueryHandlerTests()
    {
        _ongoingTrainingsQuery = Substitute.For<IOngoingTrainingsQuery>();
        _supaBaseStorage = Substitute.For<ISupaBaseStorage>();
        _handler = new GetOngoingTrainingByIdQueryHandler(_ongoingTrainingsQuery, _supaBaseStorage);

        _ongoingTrainingId = OngoingTrainingId.NewId();
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
        var ongoingTraining = OngoingTrainingReadModelFaker.Generate(id: _ongoingTrainingId);

        _ongoingTrainingsQuery
            .GetByIdAsync(_ongoingTrainingId, Arg.Any<CancellationToken>())
            .Returns(ongoingTraining);

        var query = new GetOngoingTrainingByIdQuery(_ongoingTrainingId);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(ongoingTraining);
    }
}
