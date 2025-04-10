using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;

public sealed class SearchedFood : AggregateRoot<SearchedFoodId>
{
    public string Name { get; private set; } = string.Empty;

    private SearchedFood() { }

    private SearchedFood(SearchedFoodId id, string name)
        : base(id)
    {
        Name = name;
    }

    public static Result<SearchedFood> Create(SearchedFoodId id, string name)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(SearchedFood), nameof(id))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(SearchedFood), nameof(Name))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<SearchedFood>(result.Error);
        }

        var food = new SearchedFood(id, name);

        return Result.Success(food);
    }
}
