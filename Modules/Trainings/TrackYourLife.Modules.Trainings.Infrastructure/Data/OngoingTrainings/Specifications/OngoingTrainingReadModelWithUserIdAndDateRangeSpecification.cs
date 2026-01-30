using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;

public class OngoingTrainingReadModelWithUserIdAndDateRangeSpecification(
    UserId userId,
    DateTime startDate,
    DateTime endDate
) : Specification<OngoingTrainingReadModel, OngoingTrainingId>
{
    public override Expression<Func<OngoingTrainingReadModel, bool>> ToExpression() =>
        ot => ot.UserId == userId
            && ot.FinishedOnUtc.HasValue
            && ot.FinishedOnUtc.Value >= startDate
            && ot.FinishedOnUtc.Value <= endDate;
}
