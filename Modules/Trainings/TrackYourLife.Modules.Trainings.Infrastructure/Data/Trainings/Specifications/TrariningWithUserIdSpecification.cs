using System.Linq.Expressions;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings.Specifications;

internal sealed class TrainingReadModelWithUserIdSpecification(UserId userId)
    : Specification<TrainingReadModel, TrainingId>
{
    public override Expression<Func<TrainingReadModel, bool>> ToExpression() =>
        training => training.UserId == userId;
}
