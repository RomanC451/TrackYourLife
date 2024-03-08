using System.Text.RegularExpressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.Repositories;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Persistence.Repositories;

internal sealed partial class FoodRepository : IFoodRepository
{
    private readonly ApplicationDbContext _context;

    public FoodRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private static Regex WithoutSpecialCaracterRegex()
    {
        return new Regex("[^a-zA-Z0-9 ]");
    }

    public async Task<Food?> GetByIdAsync(FoodId id, CancellationToken cancellationToken)
    {
        return await _context.Foods
            .Include(f => f.FoodServingSizes)
            .ThenInclude(fss => fss.ServingSize)
            .FirstOrDefaultAsync(food => food.Id == id, cancellationToken);
    }

    public async Task<List<Food>> GetFoodListByNameAsync(
        string name,
        CancellationToken cancellationToken
    )
    {
        return await _context.Foods
            .Include(f => f.FoodServingSizes)
            .Where(f => f.Name == name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Food>> GetFoodListByContainingNameAsync(
        string name,
        CancellationToken cancellationToken
    )
    {
        string[] searchWords = name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var predicate = PredicateBuilder.New<Food>();

        foreach (var word in searchWords)
        {
            predicate = predicate.Or(f => f.Name.ToLower().Contains(word));
        }

        var foods = await _context.Foods
            .Include(f => f.FoodServingSizes)
            .ThenInclude(fss => fss.ServingSize)
            .Where(predicate)
            .ToListAsync(cancellationToken);

        foreach (var food in foods)
        {
            food.FoodServingSizes = food.FoodServingSizes.OrderBy(fss => fss.Index).ToList();
        }

        return foods
            .Select(food => new { Food = food, MatchCount = GetMatchScore(food, name) })
            .OrderByDescending(food => food.MatchCount)
            .Select(food => food.Food)
            .ToList();
    }

    private static int GetMatchScore(Food food, string searchString)
    {
        int matchScore = 0;

        if (
            searchString.Trim().ToLower() == food.Name.ToLower()
            || searchString.Trim().ToLower() == food.BrandName.ToLower()
        )
        {
            return 100;
        }

        string[] searchWords = WithoutSpecialCaracterRegex()
            .Replace(searchString, " ")
            .Trim()
            .ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string[] foodNameWords = WithoutSpecialCaracterRegex()
            .Replace(food.Name, " ")
            .ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string[] foodBrandNameWords = WithoutSpecialCaracterRegex()
            .Replace(food.BrandName, " ")
            .ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in searchWords)
        {
            if (foodNameWords.Contains(word))
            {
                // Exact match in name, add 10 to the score
                matchScore += 10;
            }
            else if (foodBrandNameWords.Contains(word))
            {
                // Exact match in brand name, add 8 to the score
                matchScore += 8;
            }
            else if (food.BrandName.StartsWith(word, StringComparison.Ordinal))
            {
                // Word at the start of brand name with same case, add 10 to the score
                matchScore += 10;
            }
            else if (food.Name.StartsWith(word, StringComparison.OrdinalIgnoreCase))
            {
                // Word at the start of name, add 7 to the score
                matchScore += 7;
            }
            else if (food.BrandName.StartsWith(word, StringComparison.OrdinalIgnoreCase))
            {
                // Word at the start of brand name, add 5 to the score
                matchScore += 5;
            }
        }

        return matchScore;
    }

    public async Task AddAsync(Food food, CancellationToken cancellationToken)
    {
        await _context.Foods.AddAsync(food, cancellationToken);
    }

    public void Remove(Food food)
    {
        _context.Foods.Remove(food);
    }

    public async Task AddFoodListAsync(List<Food> foodList, CancellationToken cancellationToken)
    {
        foreach (var food in foodList)
        {
            Food? existingFood = await _context.Foods.FirstOrDefaultAsync(
                f => f.ApiId == food.ApiId,
                cancellationToken
            );

            if (existingFood is not null)
            {
                continue;
            }

            foreach (var foodServingSize in food.FoodServingSizes)
            {
                var existingServingSize = await _context.ServingSizes.FirstOrDefaultAsync(
                    ss => ss.ApiId == foodServingSize.ServingSize.ApiId,
                    cancellationToken
                );

                if (existingServingSize is null)
                {
                    continue;
                }

                foodServingSize.ServingSize = existingServingSize;

                foodServingSize.ServingSizeId = existingServingSize.Id;
            }

            _context.Foods.Add(food);
        }
    }
}
