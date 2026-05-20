using System.Linq.Expressions;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeCategories.Specifications;

internal sealed class YoutubeCategoryReadModelWithUserIdSpecification(UserId userId)
    : Specification<YoutubeCategoryReadModel, YoutubeCategoryId>
{
    public override Expression<Func<YoutubeCategoryReadModel, bool>> ToExpression() =>
        c => c.UserId == userId;
}
