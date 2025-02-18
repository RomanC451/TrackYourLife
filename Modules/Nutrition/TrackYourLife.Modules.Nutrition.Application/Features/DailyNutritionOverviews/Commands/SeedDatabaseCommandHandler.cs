using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Commands;

public sealed class SeedDatabaseCommandHandler(
    IDailyNutritionOverviewRepository dailyNutritionOverviewRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<SeedDatabaseCommand>
{
    public async Task<Result> Handle(
        SeedDatabaseCommand command,
        CancellationToken cancellationToken
    )
    {
        var random = new Random();
        var dailyNutritionOverviews = new List<DailyNutritionOverview>();

        var date = new DateOnly(2024, 1, 1);

        for (var i = 0; i < 365; i++)
        {
            var overview = DailyNutritionOverview
                .Create(
                    DailyNutritionOverviewId.NewId(),
                    userIdentifierProvider.UserId,
                    date,
                    random.Next(1500, 3000),
                    random.Next(150, 500),
                    random.Next(20, 150),
                    random.Next(20, 200)
                )
                .Value!;

            var nutritionalContent = new NutritionalContent()
            {
                Energy = new Energy() { Value = random.Next(1500, 3000) },
                Carbohydrates = random.Next(150, 500),
                Fat = random.Next(20, 150),
                Protein = random.Next(20, 200)
            };

            overview.AddNutritionalValues(nutritionalContent, 1);

            dailyNutritionOverviews.Add(overview);

            date = date.AddDays(1);
        }

        await dailyNutritionOverviewRepository.AddRangeAsync(
            dailyNutritionOverviews,
            cancellationToken
        );

        return Result.Success();
    }
}
