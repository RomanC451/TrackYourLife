using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries;

internal class RecipeDiaryQuery(NutritionReadDbContext dbContext)
    : GenericQuery<RecipeDiaryReadModel, NutritionDiaryId>(CreateQuery(dbContext.RecipeDiaries)),
        IRecipeDiaryQuery
{
    private static IQueryable<RecipeDiaryReadModel> CreateQuery(DbSet<RecipeDiaryReadModel> dbSet)
    {
        return dbSet.Include(de => de.Recipe);
    }

    public async Task<bool> AnyByRecipeIdAsync(
        UserId userId,
        RecipeId recipeId,
        CancellationToken cancellationToken
    )
    {
        return await query.AnyAsync(
            new RecipeDiaryReadModelWithUserIdAndRecipeIdSpecification(userId, recipeId),
            cancellationToken
        );
    }

    public async Task<IEnumerable<RecipeDiaryReadModel>> GetByDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        var list = await WhereAsync(
            new RecipeDiaryReadModelWithUserIdAndDateSpecification(userId, date),
            cancellationToken
        );

        return list.OrderBy(de => de.CreatedOnUtc);
    }

    public async Task<IEnumerable<RecipeDiaryReadModel>> GetByPeriodAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    )
    {
        var list = await WhereAsync(
            new RecipeDiaryReadModelWithUserIdAndPeriodSpecification(userId, startDate, endDate),
            cancellationToken
        );

        return list.OrderBy(de => de.CreatedOnUtc);
    }
}
