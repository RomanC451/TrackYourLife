using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;

internal sealed record GetNutritionDiariesByDateResponse(
    Dictionary<MealTypes, List<NutritionDiaryDto>> Diaries
);

internal sealed class GetNutritionDiariesByDate(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("{date}");
        Group<NutritionDiariesGroup>();
        Description(x =>
            x.Produces<GetNutritionDiariesByDateResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var date = Route<DateOnly>("date");

        return await sender
            .Send(new GetNutritionDiariesByDateQuery(date), ct)
            .ToActionResultAsync(
                (dicts) =>
                {
                    var result = new Dictionary<MealTypes, List<NutritionDiaryDto>>();

                    // Process the first dictionary
                    foreach (var kvp in dicts.Item1)
                    {
                        if (!result.TryGetValue(kvp.Key, out List<NutritionDiaryDto>? value))
                        {
                            value = ([]);
                            result[kvp.Key] = value;
                        }

                        value.AddRange(kvp.Value.Select(diary => diary.ToDto()));
                    }

                    // Process the second dictionary
                    foreach (var kvp in dicts.Item2)
                    {
                        if (!result.TryGetValue(kvp.Key, out List<NutritionDiaryDto>? value))
                        {
                            value = [];
                            result[kvp.Key] = value;
                        }

                        value.AddRange(kvp.Value.Select(diary => diary.ToDto()));
                    }

                    return TypedResults.Ok(new GetNutritionDiariesByDateResponse(result));
                }
            );
    }
}
