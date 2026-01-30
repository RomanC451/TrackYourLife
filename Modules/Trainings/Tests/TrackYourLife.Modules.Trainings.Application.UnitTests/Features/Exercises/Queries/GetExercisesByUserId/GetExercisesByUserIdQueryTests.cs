using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExercisesByUserId;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Queries.GetExercisesByUserId;

public class GetExercisesByUserIdQueryTests
{
    [Fact]
    public void Query_ShouldBeAssignableToIQuery()
    {
        var query = new GetExercisesByUserIdQuery();

        query.Should().NotBeNull();
        query.Should().BeAssignableTo<IQuery<IEnumerable<ExerciseReadModel>>>();
    }
}
