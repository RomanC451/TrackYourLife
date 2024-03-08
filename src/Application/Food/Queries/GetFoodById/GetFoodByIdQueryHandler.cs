using MapsterMapper;
using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Foods.Dtos;
using TrackYourLifeDotnet.Domain.Foods.Repositories;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Foods.Queries.GetFoodById;

public sealed class GetFoodByIdQueryHandler : IQueryHandler<GetFoodByIdQuery, GetFoodByIdResult>
{
    private readonly IFoodRepository _foodRepository;

    private readonly IMapper _mapper;

    public GetFoodByIdQueryHandler(IFoodRepository foodRepository, IMapper mapper)
    {
        _foodRepository = foodRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetFoodByIdResult>> Handle(
        GetFoodByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var food = await _foodRepository.GetByIdAsync(query.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure<GetFoodByIdResult>(DomainErrors.Food.NotFoundById(query.FoodId));
        }

        var result = new GetFoodByIdResult(_mapper.Map<FoodDto>(food));

        return Result.Success(result);
    }
}
