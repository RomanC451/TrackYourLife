using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;

public class GetTrainingsByUserIdQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ITrainingsQuery trainingsQuery
) : IQueryHandler<GetTrainingsByUserIdQuery, IEnumerable<TrainingReadModel>>
{
    public async Task<Result<IEnumerable<TrainingReadModel>>> Handle(
        GetTrainingsByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var trainings = await trainingsQuery.GetTrainingsByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(trainings);
    }
}
