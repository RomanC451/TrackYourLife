using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.FoodDiaries.Repositories;

namespace TrackYourLifeDotnet.Persistence.Repositories;

public class FoodDiaryRepository : IFoodDiaryRepository
{
    public readonly ApplicationDbContext _context;

    public FoodDiaryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FoodDiaryEntry>> GetByDateAsync(
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        return await _context.FoodDiaries
            .Where(entry => entry.Date == date)
            .ToListAsync(cancellationToken);
    }

    public async Task<FoodDiaryEntry?> GetByIdAsync(
        FoodDiaryEntryId id,
        CancellationToken cancellationToken
    )
    {
        return await _context.FoodDiaries
            .Where(entry => entry.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(FoodDiaryEntry entry, CancellationToken cancellationToken)
    {
        await _context.FoodDiaries.AddAsync(entry, cancellationToken);
    }

    public void Remove(FoodDiaryEntry entry)
    {
        _context.FoodDiaries.Remove(entry);
    }

    public void Update(FoodDiaryEntry entry)
    {
        _context.FoodDiaries.Update(entry);
    }
}
