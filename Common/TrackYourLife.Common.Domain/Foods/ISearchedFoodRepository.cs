namespace TrackYourLife.Common.Domain.Foods;

public interface ISearchedFoodRepository
{
    Task AddAsync(SearchedFood searchedFood, CancellationToken cancellationToken);

    Task<SearchedFood?> GetByNameAsync(string name, CancellationToken cancellationToken);

    void Update(SearchedFood searchedFood);
}
