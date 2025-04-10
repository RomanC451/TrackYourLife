using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

public sealed class DailyNutritionOverview : Entity<DailyNutritionOverviewId>
{
    public UserId UserId { get; private set; } = UserId.Empty;
    public DateOnly Date { get; private set; }
    public NutritionalContent NutritionalContent { get; private set; } = new();

    public float CaloriesGoal { get; private set; }
    public float CarbohydratesGoal { get; private set; }
    public float FatGoal { get; private set; }
    public float ProteinGoal { get; private set; }

    private DailyNutritionOverview() { }

    private DailyNutritionOverview(
        DailyNutritionOverviewId id,
        UserId userId,
        DateOnly date,
        NutritionalContent nutritionalContent,
        float caloriesGoal,
        float carbohydratesGoal,
        float fatGoal,
        float proteinGoal
    )
        : base(id)
    {
        UserId = userId;
        Date = date;
        NutritionalContent = nutritionalContent;
        CaloriesGoal = caloriesGoal;
        CarbohydratesGoal = carbohydratesGoal;
        FatGoal = fatGoal;
        ProteinGoal = proteinGoal;
    }

    public static Result<DailyNutritionOverview> Create(
        DailyNutritionOverviewId id,
        UserId userId,
        DateOnly date,
        float caloriesGoal,
        float carbohydratesGoal,
        float fatGoal,
        float proteinGoal
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(DailyNutritionOverview), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(DailyNutritionOverview), nameof(userId))
            ),
            Ensure.NotEmpty(
                date,
                DomainErrors.ArgumentError.Empty(nameof(DailyNutritionOverview), nameof(date))
            ),
            Ensure.NotNegative(
                caloriesGoal,
                DomainErrors.ArgumentError.Negative(
                    nameof(DailyNutritionOverview),
                    nameof(caloriesGoal)
                )
            ),
            Ensure.NotNegative(
                carbohydratesGoal,
                DomainErrors.ArgumentError.Negative(
                    nameof(DailyNutritionOverview),
                    nameof(carbohydratesGoal)
                )
            ),
            Ensure.NotNegative(
                fatGoal,
                DomainErrors.ArgumentError.Negative(nameof(DailyNutritionOverview), nameof(fatGoal))
            ),
            Ensure.NotNegative(
                proteinGoal,
                DomainErrors.ArgumentError.Negative(
                    nameof(DailyNutritionOverview),
                    nameof(proteinGoal)
                )
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<DailyNutritionOverview>(result.Error);
        }

        var nutritionalContent = new NutritionalContent
        {
            Energy = new Energy { Unit = "Kcal", Value = 0 },
        };

        var overview = new DailyNutritionOverview(
            id,
            userId,
            date,
            nutritionalContent,
            caloriesGoal,
            carbohydratesGoal,
            fatGoal,
            proteinGoal
        );

        return Result.Success(overview);
    }

    public void AddNutritionalValues(NutritionalContent nutritionalContent, float quantity)
    {
        NutritionalContent.AddNutritionalValues(
            nutritionalContent.MultiplyNutritionalValues(quantity)
        );
    }

    public void SubtractNutritionalValues(NutritionalContent nutritionalContent, float quantity)
    {
        NutritionalContent.SubtractNutritionalValues(
            nutritionalContent.MultiplyNutritionalValues(quantity)
        );
    }

    public Result UpdateCaloriesGoal(float newCaloriesGoal)
    {
        var result = Ensure.Positive(
            newCaloriesGoal,
            DomainErrors.ArgumentError.NotPositive(
                nameof(DailyNutritionOverview),
                nameof(newCaloriesGoal)
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        CaloriesGoal = newCaloriesGoal;

        return Result.Success();
    }

    public Result UpdateCarbohydratesGoal(float newCarbohydratesGoal)
    {
        var result = Ensure.Positive(
            newCarbohydratesGoal,
            DomainErrors.ArgumentError.NotPositive(
                nameof(DailyNutritionOverview),
                nameof(newCarbohydratesGoal)
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        CarbohydratesGoal = newCarbohydratesGoal;

        return Result.Success();
    }

    public Result UpdateProteinGoal(float newProteinGoal)
    {
        var result = Ensure.Positive(
            newProteinGoal,
            DomainErrors.ArgumentError.NotPositive(
                nameof(DailyNutritionOverview),
                nameof(newProteinGoal)
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        ProteinGoal = newProteinGoal;

        return Result.Success();
    }

    public Result UpdateFatGoal(float newFatGoal)
    {
        var result = Ensure.Positive(
            newFatGoal,
            DomainErrors.ArgumentError.NotPositive(
                nameof(DailyNutritionOverview),
                nameof(newFatGoal)
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        FatGoal = newFatGoal;

        return Result.Success();
    }
}
