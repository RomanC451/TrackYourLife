// using MediatR;
// using TrackYourLifeDotnet.Application.Core.Abstractions.Services;
// using TrackYourLifeDotnet.Domain.Foods.DomainEvents;
// using TrackYourLifeDotnet.Domain.Foods.Repositories;
// using TrackYourLifeDotnet.Domain.Repositories;

// namespace TrackYourLifeDotnet.Application.Foods.Events;

// public sealed class FoodSearchedDomainEventHandler : INotificationHandler<FoodSearchedDomainEvent>
// {
//     private readonly ISearchedFoodRepository _searchedFoodRepository;

//     private readonly IFoodRepository _foodRepository;

//     private readonly IUnitOfWork _unitOfWork;

//     private readonly IFoodApiService _foodApiService;

//     public FoodSearchedDomainEventHandler(
//         ISearchedFoodRepository searchedFoodRepository,
//         IUnitOfWork unitOfWork,
//         IFoodApiService foodApiService,
//         IFoodRepository foodRepository
//     )
//     {
//         _searchedFoodRepository = searchedFoodRepository;
//         _unitOfWork = unitOfWork;
//         _foodApiService = foodApiService;
//         _foodRepository = foodRepository;
//     }

//     public async Task Handle(
//         FoodSearchedDomainEvent notification,
//         CancellationToken cancellationToken
//     )
//     {
//         // var searchedFood = await _searchedFoodRepository.GetByNameAsync(
//         //     notification.FoodName,
//         //     cancellationToken
//         // );

//         // if (searchedFood is null)
//         //     // !!! TODO: Handle this case
//         //     return;

//         // if (searchedFood.FullySearched)
//         //     return;

//         // List<Food> foodList = await _foodApiService.GetFoodListAsync(
//         //     notification.FoodName,
//         //     cancellationToken
//         // );

//         // await _foodRepository.AddFoodListAsync(foodList, cancellationToken);

//         // searchedFood.FullySearched = true;

//         // _searchedFoodRepository.Update(searchedFood);

//         // await _unitOfWork.SaveChangesAsync(cancellationToken);

//         throw new NotImplementedException();
//     }
// }
