using MapsterMapper;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Contracts.Common;
using TrackYourLife.Common.Contracts.Foods;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.Foods.Queries.GetFoodList;

public sealed class GetFoodListQueryHandler
    : IQueryHandler<GetFoodListQuery, FoodListResponse>
{
    private readonly IFoodApiService _foodApiService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IFoodRepository _foodRepository;

    private readonly ISearchedFoodRepository _searchedFoodRepository;

    private readonly IMapper _mapper;

    public GetFoodListQueryHandler(
        IFoodApiService foodApiService,
        IUnitOfWork unitOfWork,
        IFoodRepository foodRepository,
        ISearchedFoodRepository searchedFoodRepository,
        IMapper mapper
    )
    {
        _foodApiService = foodApiService;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
        _searchedFoodRepository = searchedFoodRepository;
        _mapper = mapper;
    }

    public async Task<Result<FoodListResponse>> Handle(
        GetFoodListQuery query,
        CancellationToken cancellationToken
    )
    {
        List<Food> foodList;

        var searchedFood = await _searchedFoodRepository.GetByNameAsync(
            query.SearchParam.ToLower(),
            cancellationToken
        );

        if (searchedFood is not null)
        {
            foodList = await _foodRepository.GetFoodListByContainingNameAsync(
                query.SearchParam,
                cancellationToken
            );
        }
        else
        {
            Result<List<Food>> response = await _foodApiService.GetFoodListAsync(
                query.SearchParam,
                query.Page,
                cancellationToken
            );

            if (response.IsFailure)
            {
                if (response.Error.Code.Contains("NotFound"))
                {
                    return Result.Failure<FoodListResponse>(
                        DomainErrors.Food.NotFoundByName(query.SearchParam)
                    );
                }
                else
                {
                    return Result.Failure<FoodListResponse>(response.Error);
                }
            }

            foodList = response.Value;

            if (foodList.Count == 0)
            {
                return Result.Failure<FoodListResponse>(
                    DomainErrors.Food.NotFoundByName(query.SearchParam)
                );
            }

            //TODO: Make this async
            await AddFoodToDB(query.SearchParam, foodList, cancellationToken);

            foodList = await _foodRepository.GetFoodListByContainingNameAsync(
                query.SearchParam,
                cancellationToken
            );
        }

        if (foodList.Count == 0)
        {
            return Result.Failure<FoodListResponse>(
                DomainErrors.Food.NotFoundByName(query.SearchParam)
            );
        }

        List<FoodResponse> foods = foodList.Select(_mapper.Map<FoodResponse>).ToList();

        PagedList<FoodResponse> pagedFoodList = PagedList<FoodResponse>.Create(
            foods,
            query.Page,
            query.PageSize
        );

        if (pagedFoodList.Items.Count == 0)
        {
            return Result.Failure<FoodListResponse>(
                DomainErrors.Food.PageOutOfIndex(query.Page, pagedFoodList.MaxPage)
            );
        }

        return Result.Success(new FoodListResponse(List: pagedFoodList));
    }

    private async Task AddFoodToDB(
        string foodName,
        List<Food> foodList,
        CancellationToken cancellationToken
    )
    {
        await _foodRepository.AddFoodListAsync(foodList, cancellationToken);

        var searchedFood = new SearchedFood(SearchedFoodId.NewId(), foodName.ToLower());

        await _searchedFoodRepository.AddAsync(searchedFood, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
