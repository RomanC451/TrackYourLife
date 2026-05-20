using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeCategories;

internal sealed class YoutubeCategoriesQuery(YoutubeReadDbContext dbContext)
    : GenericQuery<YoutubeCategoryReadModel, YoutubeCategoryId>(dbContext.YoutubeCategories),
        IYoutubeCategoriesQuery
{
    public async Task<IReadOnlyList<YoutubeCategoryReadModel>> ListByUserIdOrderedAsync(
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
}