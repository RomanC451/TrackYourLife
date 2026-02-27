using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingsByUserId;

public class GetTrainingsByUserIdQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ITrainingsQuery _trainingsQuery;
    private readonly GetTrainingsByUserIdQueryHandler _handler;

    private readonly UserId _userId;

    public GetTrainingsByUserIdQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _trainingsQuery = Substitute.For<ITrainingsQuery>();
        _handler = new GetTrainingsByUserIdQueryHandler(_userIdentifierProvider, _trainingsQuery);

        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenNoTrainingsFound_ShouldReturnEmptyCollection()
    {
        // Arrange
        _trainingsQuery
            .GetTrainingsByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns([]);

        var query = new GetTrainingsByUserIdQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenTrainingsFound_ShouldReturnTrainings()
    {
        // Arrange
        var trainings = new List<TrainingReadModel>
        {
            TrainingReadModelFaker.Generate(),
            TrainingReadModelFaker.Generate(),
        };

        _trainingsQuery
            .GetTrainingsByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(trainings);

        var query = new GetTrainingsByUserIdQuery();

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(trainings);
    }

    [Fact]
    public async Task Handle_ShouldUseCurrentUserId()
    {
        // Arrange
        var trainings = new List<TrainingReadModel> { TrainingReadModelFaker.Generate() };

        _trainingsQuery
            .GetTrainingsByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(trainings);

        var query = new GetTrainingsByUserIdQuery();

        // Act
        await _handler.Handle(query, default);

        // Assert
        await _trainingsQuery
            .Received(1)
            .GetTrainingsByUserIdAsync(_userId, Arg.Any<CancellationToken>());
    }
}

