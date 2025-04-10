using MassTransit;
using MediatR;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Events;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Events;

internal sealed class GoalCreatedDomainEventHandler(IGoalQuery goalQuery, IBus bus)
    : INotificationHandler<GoalCreatedDomainEvent>
{
    public async Task Handle(
        GoalCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var goal = await goalQuery.GetByIdAsync(notification.GoalId, cancellationToken);

        if (goal is null)
        {
            return;
        }

        var nutritionGoalUpdatedIntegrationEvent = new NutritionGoalUpdatedIntegrationEvent(
            notification.UserId,
            goal.StartDate,
            goal.EndDate,
            goal.Value,
            goal.Type
        );

        await bus.Publish(nutritionGoalUpdatedIntegrationEvent, cancellationToken);
    }
}
