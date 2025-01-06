using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Tokens;

public interface ITokenRepository
{
    Task<Token?> GetByValueAsync(string value, CancellationToken cancellationToken);

    Task<List<Token>> GetByUserIdAndTypeAsync(
        UserId id,
        TokenType tokenType,
        CancellationToken cancellationToken
    );
    Task AddAsync(Token token, CancellationToken cancellationToken);

    void Remove(Token token);

    void Update(Token token);
}
