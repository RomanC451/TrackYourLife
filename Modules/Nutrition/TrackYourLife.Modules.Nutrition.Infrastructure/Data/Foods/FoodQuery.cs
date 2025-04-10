using System.Text.RegularExpressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;

internal sealed class FoodQuery(NutritionReadDbContext context)
    : GenericQuery<FoodReadModel, FoodId>(CreateQuery(context.Foods)),
        IFoodQuery
{
    private static IQueryable<FoodReadModel> CreateQuery(DbSet<FoodReadModel> dbSet)
    {
        return dbSet.Include(f => f.FoodServingSizes).ThenInclude(fss => fss.ServingSize);
    }

    public async Task<IEnumerable<FoodReadModel>> SearchFoodAsync(
        string searchTerm,
        CancellationToken cancellationToken
    )
    {
        string[] searchWords = searchTerm
            .ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        ExpressionStarter<FoodReadModel> predicate = PredicateBuilder.New<FoodReadModel>();

        foreach (var word in searchWords)
        {
            predicate = predicate.Or(f => f.Name.ToLower().Contains(word));
        }

        return await query
            .Where(f => f.SearchVector.Matches(EF.Functions.PlainToTsQuery("english", searchTerm)))
            .Select(f => new
            {
                Food = f,
                Rank = f.SearchVector.Rank(EF.Functions.PlainToTsQuery("english", searchTerm)),
            })
            .OrderByDescending(f => f.Rank)
            .Select(f => f.Food)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FoodReadModel>> GetFoodsPartOfAsync(
        IEnumerable<FoodId> foodIds,
        CancellationToken cancellationToken
    )
    {
        return await query.Where(f => foodIds.Contains(f.Id)).ToListAsync(cancellationToken);
    }
}
