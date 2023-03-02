using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Persistence;
using TrackYourLifeDotnet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLife.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    public readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return await _context.Users.FirstOrDefaultAsync(
            user => user.Email == email,
            cancellationToken
        );
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken)
    {
        return !await _context.Users.AnyAsync(user => user.Email == email, cancellationToken);
    }

    public void Add(User user) => _context.Users.Add(user);

    public void Update(User user) => _context.Users.Update(user);

    public void Remove(User user) => _context.Users.Remove(user);
}
