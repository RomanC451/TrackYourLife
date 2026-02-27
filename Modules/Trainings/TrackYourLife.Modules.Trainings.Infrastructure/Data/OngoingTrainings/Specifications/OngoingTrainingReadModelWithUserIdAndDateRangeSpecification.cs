using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;

/// <summary>
/// Filters by user and date range using a half-open interval [startInclusive, endExclusive)
/// so that the full end date is included without relying on end-of-day time.
/// </summary>
public class OngoingTrainingReadModelWithUserIdAndDateRangeSpecification(
    UserId userId,
    DateTime startInclusive,
    DateTime endExclusive
) : Specification<OngoingTrainingReadModel, OngoingTrainingId>
{
    public override Expression<Func<OngoingTrainingReadModel, bool>> ToExpression() =>
        ot => ot.UserId == userId
            && ot.FinishedOnUtc.HasValue
            && ot.FinishedOnUtc.Value >= startInclusive
            && ot.FinishedOnUtc.Value < endExclusive;
}
