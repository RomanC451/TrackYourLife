using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExercisesByUserId;

public class GetExercisesByUserIdQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExercisesQuery exerciseQuery
) : IQueryHandler<GetExercisesByUserIdQuery, IEnumerable<ExerciseReadModel>>
{
    public async Task<Result<IEnumerable<ExerciseReadModel>>> Handle(
        GetExercisesByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var exercises = await exerciseQuery.GetEnumerableByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(exercises.OrderBy(e => e.CreatedOnUtc).AsEnumerable());
    }
}
