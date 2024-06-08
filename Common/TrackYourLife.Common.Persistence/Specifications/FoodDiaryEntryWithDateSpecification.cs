using System.Linq.Expressions;
using TrackYourLife.Common.Domain.FoodDiaries;

namespace TrackYourLife.Common.Persistence.Specifications;

internal class FoodDiaryEntryWithDateSpecification : Specification<FoodDiaryEntry, FoodDiaryEntryId>
{
    private readonly DateOnly _date;

    public FoodDiaryEntryWithDateSpecification(DateOnly date) => _date = date;

    public override Expression<Func<FoodDiaryEntry, bool>> ToExpression() =>
        entry => entry.Date == _date;
}
