using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

public sealed class GetOngoingTrainingByIdQueryHandler(IOngoingTrainingsQuery ongoingTrainingsQuery)
    : IQueryHandler<GetOngoingTrainingByIdQuery, OngoingTrainingReadModel>
{
    public async Task<Result<OngoingTrainingReadModel>> Handle(
        GetOngoingTrainingByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsQuery.GetByIdAsync(
            request.Id,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure<OngoingTrainingReadModel>(
                OngoingTrainingErrors.NotFoundById(request.Id)
            );
        }

        return ongoingTraining;
    }
}
