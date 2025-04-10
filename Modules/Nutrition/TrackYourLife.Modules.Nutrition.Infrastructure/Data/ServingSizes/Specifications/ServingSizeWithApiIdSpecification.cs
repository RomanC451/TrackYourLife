using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes.Specifications;

internal sealed class ServingSizeWithApiIdSpecification(long ApiId)
    : Specification<ServingSize, ServingSizeId>
{
    public override Expression<Func<ServingSize, bool>> ToExpression() => ss => ss.ApiId == ApiId;
}
