// using MassTransit;
// using MediatR;
// using TrackYourLife.Modules.Users.Application.Features.Goals.Events;
// using TrackYourLife.Modules.Users.Domain.Features.Goals;
// using TrackYourLife.Modules.Users.Domain.Features.Goals.Events;
// using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
// using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;
// using TrackYourLife.SharedLib.Domain.Enums;
// using TrackYourLife.SharedLib.Domain.Ids;

// namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Events;

// public sealed class GoalCreatedDomainEventHandlerTests
// {
//     private readonly GoalCreatedDomainEventHandler sut;
//     private readonly IGoalQuery goalQuery = Substitute.For<IGoalQuery>();
//     private readonly IBus bus = Substitute.For<IBus>();

//     public GoalCreatedDomainEventHandlerTests()
//     {
//         sut = new GoalCreatedDomainEventHandler(goalQuery, bus);
//     }

//     [Fact]
//     public async Task Handle_WhenGoalExists_ShouldPublishIntegrationEvent()
//     {
//         // Arrange
//         var userId = UserId.NewId();
//         var goalId = GoalId.NewId();
//         var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
//         var endDate = startDate.AddDays(1);
//         var value = 2000;
//         var type = GoalType.Calories;

//         var goal = GoalFaker.GenerateReadModel(
//             id: goalId,
//             userId: userId,
//             type: type,
//             value: value,
//             startDate: startDate,
//             endDate: endDate
//         );

//         var domainEvent = new GoalCreatedDomainEvent(userId, goalId);

//         goalQuery.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns(goal);

//         // Act
//         await sut.Handle(domainEvent, CancellationToken.None);

//         // Assert
//         await bus.Received(1)
//             .Publish(
//                 Arg.Is<NutritionGoalUpdatedIntegrationEvent>(e =>
//                     e.UserId == userId
//                     && e.StartDate == startDate
//                     && e.EndDate == endDate
//                     && Math.Abs(e.Value - value) < 0.0001
//                     && e.Type == type
//                 ),
//                 Arg.Any<CancellationToken>()
//             );
//     }

//     [Fact]
//     public async Task Handle_WhenGoalDoesNotExist_ShouldNotPublishIntegrationEvent()
//     {
//         // Arrange
//         var userId = UserId.NewId();
//         var goalId = GoalId.NewId();

//         var domainEvent = new GoalCreatedDomainEvent(userId, goalId);

//         goalQuery.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns((GoalReadModel?)null);

//         // Act
//         await sut.Handle(domainEvent, CancellationToken.None);

//         // Assert
//         await bus.DidNotReceive()
//             .Publish(Arg.Any<NutritionGoalUpdatedIntegrationEvent>(), Arg.Any<CancellationToken>());
//     }

//     [Fact]
//     public async Task Handle_WhenGoalExists_ShouldQueryGoalExactlyOnce()
//     {
//         // Arrange
//         var userId = UserId.NewId();
//         var goalId = GoalId.NewId();
//         var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
//         var endDate = startDate.AddDays(1);

//         var goal = GoalFaker.GenerateReadModel(
//             id: goalId,
//             userId: userId,
//             startDate: startDate,
//             endDate: endDate
//         );

//         var domainEvent = new GoalCreatedDomainEvent(userId, goalId);

//         goalQuery.GetByIdAsync(goalId, Arg.Any<CancellationToken>()).Returns(goal);

//         // Act
//         await sut.Handle(domainEvent, CancellationToken.None);

//         // Assert
//         await goalQuery.Received(1).GetByIdAsync(goalId, Arg.Any<CancellationToken>());
//     }
// }
