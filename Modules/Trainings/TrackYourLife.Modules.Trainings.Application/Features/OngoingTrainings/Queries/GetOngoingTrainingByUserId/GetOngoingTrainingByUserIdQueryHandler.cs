using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

public sealed class GetOngoingTrainingByUserIdQueryHandler(
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetOngoingTrainingByUserIdQuery, OngoingTrainingReadModel>
{
    public async Task<Result<OngoingTrainingReadModel>> Handle(
        GetOngoingTrainingByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsQuery.GetUnfinishedByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure<OngoingTrainingReadModel>(OngoingTrainingErrors.NotFound);
        }

        return Result.Success(ongoingTraining);
    }
}
