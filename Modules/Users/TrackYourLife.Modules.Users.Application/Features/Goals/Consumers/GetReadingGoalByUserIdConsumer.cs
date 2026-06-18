using MassTransit;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Consumers;

public sealed class GetReadingGoalByUserIdConsumer(IGoalQuery goalQuery)
    : IConsumer<GetReadingGoalByUserIdRequest>
{
    public async Task Consume(ConsumeContext<GetReadingGoalByUserIdRequest> context)
    {
        var goal = await goalQuery.GetGoalByTypeAndDateAsync(
            context.Message.UserId,
            GoalType.ReadingPages,
            context.Message.Date,
            context.CancellationToken
        );

        if (goal is null)
        {
            await context.RespondAsync(
                new GetReadingGoalByUserIdResponse(null, [GoalErrors.NotExisting(GoalType.ReadingPages)])
            );
            return;
        }

        await context.RespondAsync(new GetReadingGoalByUserIdResponse(goal.Value, []));
    }
}
