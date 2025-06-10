using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings.Specifications;

internal sealed class OngoingTrainingWithTrainingIdSpecification(TrainingId trainingId)
    : Specification<OngoingTraining, OngoingTrainingId>
{
    public override Expression<Func<OngoingTraining, bool>> ToExpression() =>
        x => x.Training.Id == trainingId;
}
