using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Common.Domain.ServingSizes;
using TrackYourLife.Common.Domain.Users;

namespace TrackYourLife.Common.Domain.FoodDiaries;

public class FoodDiaryEntry : Entity<FoodDiaryEntryId>, IAuditableEntity
{
    public FoodDiaryEntry(
        FoodDiaryEntryId id,
        UserId userId,
        Food food,
        float quantity,
        DateOnly date,
        MealTypes mealType,
        ServingSize servingSize
    )
        : base(id)
    {
        UserId = userId;
        Food = food;
        Quantity = quantity;
        Date = date;
        MealType = mealType;
        ServingSize = servingSize;
    }

    protected FoodDiaryEntry()
        : base() { }

    public UserId UserId { get; set; } = new();
    public Food Food { get; set; } = new();
    public float Quantity { get; set; }
    public ServingSize ServingSize { get; set; } = new();
    public DateOnly Date { get; set; }
    public MealTypes MealType { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }
}
