using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExercisePerformance;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Queries.GetExercisePerformance;

public class GetExercisePerformanceQueryTests
{
    [Fact]
    public void Query_WithDefaultValues_ShouldReflectDefaults()
    {
        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            CalculationMethod: PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        query.Should().NotBeNull();
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
        query.CalculationMethod.Should().Be(PerformanceCalculationMethod.Sequential);
        query.Should().BeAssignableTo<IQuery<PagedList<ExercisePerformanceDto>>>();
    }
}
