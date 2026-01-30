using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingsByUserId;

public class GetTrainingsByUserIdQueryTests
{
    [Fact]
    public void Query_ShouldBeAssignableToIQuery()
    {
        var query = new GetTrainingsByUserIdQuery();

        query.Should().NotBeNull();
        query.Should().BeAssignableTo<IQuery<IEnumerable<TrainingReadModel>>>();
    }
}
