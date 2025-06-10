using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;

internal sealed class OngoingTrainingReadModelWithTrainingIdAndNotFinishedSpecification(
    TrainingId trainingId
) : Specification<OngoingTrainingReadModel, OngoingTrainingId>
{
    public override Expression<Func<OngoingTrainingReadModel, bool>> ToExpression()
    {
        return x => x.Training.Id == trainingId && x.FinishedOnUtc == null;
    }
}

internal sealed class OngoingTrainingWithTrainingIdAndNotFinishedSpecification(
    TrainingId trainingId
) : Specification<OngoingTraining, OngoingTrainingId>
{
    public override Expression<Func<OngoingTraining, bool>> ToExpression()
    {
        return x => x.Training.Id == trainingId && x.FinishedOnUtc == null;
    }
}
