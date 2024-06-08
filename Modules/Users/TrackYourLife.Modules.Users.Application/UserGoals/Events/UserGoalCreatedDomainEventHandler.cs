// using MediatR;
// using TrackYourLifeDotnet.Domain.Repositories;
// using TrackYourLifeDotnet.Domain.UserGoals;

// namespace TrackYourLifeDotnet.Application.UserGoals.Events;

// public sealed record UserGoalCreatedEventHandler : INotificationHandler<UserGoalCreatedDomainEvent>
// {
//     private readonly IUserGoalRepository _userGoalRepository;
//     private readonly IUnitOfWork _unitOfWork;

//     public UserGoalCreatedEventHandler(
//         IUserGoalRepository userGoalRepository,
//         IUnitOfWork unitOfWork
//     )
//     {
//         _userGoalRepository = userGoalRepository;
//         _unitOfWork = unitOfWork;
//     }

//     public async Task Handle(
//         UserGoalCreatedDomainEvent domainEvent,
//         CancellationToken cancellationToken
//     )
//     {
//         List<UserGoal> activeGoals = await _userGoalRepository.GetActiveGoalsByTypeAsync(
//             domainEvent.UserId,
//             domainEvent.Type,
//             cancellationToken
//         );

//         foreach (UserGoal activeGoal in activeGoals)
//         {
//             if (activeGoal.Id != domainEvent.UserGoalId)
//             {
//                 activeGoal.EndDate = domainEvent.StartDate;
//                 _userGoalRepository.Update(activeGoal);
//             }
//         }

//         await _unitOfWork.SaveChangesAsync(cancellationToken);
//     }
// }
