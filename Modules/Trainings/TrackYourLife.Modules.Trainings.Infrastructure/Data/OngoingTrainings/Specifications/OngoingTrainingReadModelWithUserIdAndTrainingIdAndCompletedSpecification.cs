using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;

internal sealed class OngoingTrainingReadModelWithUserIdAndTrainingIdAndCompletedSpecification(
    UserId userId,
    TrainingId trainingId
) : Specification<OngoingTrainingReadModel, OngoingTrainingId>
{
    public override Expression<Func<OngoingTrainingReadModel, bool>> ToExpression() =>
        ot =>
            ot.UserId == userId
            && ot.Training.Id == trainingId
            && ot.FinishedOnUtc.HasValue;
}
