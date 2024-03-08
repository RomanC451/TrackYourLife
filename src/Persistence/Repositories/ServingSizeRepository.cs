using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.Repositories;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Persistence.Repositories;

public class ServingSizeRepository : IServingSizeRepository
{
    private readonly ApplicationDbContext _context;

    public ServingSizeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServingSize?> GetByIdAsync(
        ServingSizeId id,
        CancellationToken cancellationToken
    )
    {
        return await _context.ServingSizes.FirstOrDefaultAsync(
            servingSize => servingSize.Id == id,
            cancellationToken
        );
    }
}
