using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.Repositories;

namespace TrackYourLifeDotnet.Persistence.Repositories
{
    public class SearchedFoodRepository : ISearchedFoodRepository
    {
        private readonly ApplicationDbContext _context;

        public SearchedFoodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SearchedFood searchedFood, CancellationToken cancellationToken)
        {
            await _context.SearchedFood.AddAsync(searchedFood, cancellationToken);
        }

        public async Task<SearchedFood?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken
        )
        {
            return await _context.SearchedFood.FirstOrDefaultAsync(
                searchedFood => searchedFood.Name == name,
                cancellationToken
            );
        }

        public void Update(SearchedFood searchedFood)
        {
            _context.SearchedFood.Update(searchedFood);
        }
    }
}
