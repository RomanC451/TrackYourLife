using MapsterMapper;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.Foods;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.Foods.Queries.GetFoodById;

public sealed class GetFoodByIdQueryHandler : IQueryHandler<GetFoodByIdQuery, FoodResponse>
{
    private readonly IFoodRepository _foodRepository;

    private readonly IMapper _mapper;

    public GetFoodByIdQueryHandler(IFoodRepository foodRepository, IMapper mapper)
    {
        _foodRepository = foodRepository;
        _mapper = mapper;
    }

    public async Task<Result<FoodResponse>> Handle(
        GetFoodByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var food = await _foodRepository.GetByIdAsync(query.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure<FoodResponse>(DomainErrors.Food.NotFoundById(query.FoodId));
        }

        return Result.Success(_mapper.Map<FoodResponse>(food));
    }
}
