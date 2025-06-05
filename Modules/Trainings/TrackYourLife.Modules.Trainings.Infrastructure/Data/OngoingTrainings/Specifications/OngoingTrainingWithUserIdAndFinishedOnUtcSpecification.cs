using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;

public class OngoingTrainingReadModelWithUserIdAndFinishedOnUtcSpecification(
    UserId userId,
    DateTime? finishedOnUtc
) : Specification<OngoingTrainingReadModel, OngoingTrainingId>
{
    public override Expression<Func<OngoingTrainingReadModel, bool>> ToExpression() =>
        ot => ot.UserId == userId && ot.FinishedOnUtc == finishedOnUtc;
}

public class OngoingTrainingWithUserIdAndFinishedOnUtcSpecification(
    UserId userId,
    DateTime? finishedOnUtc
) : Specification<OngoingTraining, OngoingTrainingId>
{
    public override Expression<Func<OngoingTraining, bool>> ToExpression() =>
        ot => ot.UserId == userId && ot.FinishedOnUtc == finishedOnUtc;
}
