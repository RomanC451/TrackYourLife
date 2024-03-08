using MapsterMapper;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.Dtos;
using TrackYourLifeDotnet.Domain.Foods.Repositories;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Foods.Queries.GetFoodList;

public sealed class GetFoodListQueryHandler : IQueryHandler<GetFoodListQuery, GetFoodListResult>
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

    public async Task<Result<GetFoodListResult>> Handle(
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
            Result<List<Domain.Foods.Food>> response = await _foodApiService.GetFoodListAsync(
                query.SearchParam,
                query.Page,
                cancellationToken
            );

            if (response.IsFailure)
            {
                if (response.Error.Code.Contains("NotFound"))
                {
                    return Result.Failure<GetFoodListResult>(
                        DomainErrors.Food.NotFoundByName(query.SearchParam)
                    );
                }
                else
                {
                    return Result.Failure<GetFoodListResult>(response.Error);
                }
            }

            foodList = response.Value;

            if (foodList.Count == 0)
            {
                return Result.Failure<GetFoodListResult>(
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
            return Result.Failure<GetFoodListResult>(
                DomainErrors.Food.NotFoundByName(query.SearchParam)
            );
        }

        List<FoodDto> foodDtos = foodList.Select(_mapper.Map<FoodDto>).ToList();

        PagedList<FoodDto> pagedFoodList = PagedList<FoodDto>.Create(
            foodDtos,
            query.Page,
            query.PageSize
        );

        if (pagedFoodList.Items.Count == 0)
        {
            return Result.Failure<GetFoodListResult>(
                DomainErrors.Food.PageOutOfIndex(query.Page, pagedFoodList.MaxPage)
            );
        }

        return new GetFoodListResult(pagedFoodList);
    }

    private async Task AddFoodToDB(
        string foodName,
        List<Domain.Foods.Food> foodList,
        CancellationToken cancellationToken
    )
    {
        await _foodRepository.AddFoodListAsync(foodList, cancellationToken);

        var searchedFood = new SearchedFood(SearchedFoodId.NewId(), foodName.ToLower());

        await _searchedFoodRepository.AddAsync(searchedFood, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
