using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeCategories;

internal sealed class YoutubeCategoriesRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<YoutubeCategory, YoutubeCategoryId>(dbContext.YoutubeCategories),
        IYoutubeCategoriesRepository
{
    public async Task<IReadOnlyList<YoutubeCategory>> ListByUserIdOrderedAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .YoutubeCategories.Where(c => c.UserId == userId)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.YoutubeCategories.CountAsync(
            c => c.UserId == userId,
            cancellationToken
        );
    }

    public async Task<bool> ExistsByUserIdAndNameIgnoreCaseAsync(
        UserId userId,
        string name,
        YoutubeCategoryId? excludeId,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = name.Trim().ToUpperInvariant();
        var query = dbContext.YoutubeCategories.Where(c =>
            c.UserId == userId && c.Name.ToUpper() == normalized
        );

        if (excludeId is not null)
        {
            query = query.Where(c => c.Id != excludeId);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
