using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutHistory;

public class GetWorkoutHistoryQueryTests
{
    [Fact]
    public void Query_WithDefaultValues_ShouldReflectDefaults()
    {
        var query = new GetWorkoutHistoryQuery(StartDate: null, EndDate: null);

        query.Should().NotBeNull();
        query.StartDate.Should().BeNull();
        query.EndDate.Should().BeNull();
        query.Should().BeAssignableTo<IQuery<IEnumerable<WorkoutHistoryDto>>>();
    }

    [Fact]
    public void Query_WithDateRange_ShouldReflectValues()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var query = new GetWorkoutHistoryQuery(StartDate: startDate, EndDate: endDate);

        query.StartDate.Should().Be(startDate);
        query.EndDate.Should().Be(endDate);
    }
}
