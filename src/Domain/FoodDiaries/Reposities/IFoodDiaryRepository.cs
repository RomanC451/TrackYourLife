namespace TrackYourLifeDotnet.Domain.FoodDiaries.Repositories;

public interface IFoodDiaryRepository
{
    Task<List<FoodDiaryEntry>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken);
    Task<FoodDiaryEntry?> GetByIdAsync(FoodDiaryEntryId id, CancellationToken cancellationToken);
    Task AddAsync(FoodDiaryEntry entry, CancellationToken cancellationToken);
    void Remove(FoodDiaryEntry entry);
    void Update(FoodDiaryEntry entry);
}
