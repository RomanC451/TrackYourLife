using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Data.Tokens;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Data.Tokens;

public class TokenRepositoryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private TokenRepository _repository = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _repository = new TokenRepository(WriteDbContext);
    }

    [Fact]
    public async Task GetByValueAsync_WhenTokenExists_ReturnsToken()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        var token = TokenFaker.Generate(
            userId: user.Id,
            type: TokenType.RefreshToken,
            value: "test-token-value"
        );
        await WriteDbContext.Tokens.AddAsync(token);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.GetByValueAsync(
                "test-token-value",
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(token.Id);
            result.Value.Should().Be("test-token-value");
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByValueAsync_WhenTokenDoesNotExist_ReturnsNull()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.GetByValueAsync(
                "non-existent-token",
                CancellationToken.None
            );

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndTypeAsync_WhenTokensExist_ReturnsTokens()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        var token1 = TokenFaker.Generate(
            userId: user.Id,
            type: TokenType.RefreshToken,
            value: "token-1"
        );
        var token2 = TokenFaker.Generate(
            userId: user.Id,
            type: TokenType.RefreshToken,
            value: "token-2"
        );
        await WriteDbContext.Tokens.AddRangeAsync(token1, token2);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.GetByUserIdAndTypeAsync(
                user.Id,
                TokenType.RefreshToken,
                CancellationToken.None
            );

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Id == token1.Id);
            result.Should().Contain(t => t.Id == token2.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndTypeAsync_WhenNoTokensExist_ReturnsEmptyList()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.GetByUserIdAndTypeAsync(
                user.Id,
                TokenType.RefreshToken,
                CancellationToken.None
            );

            // Assert
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
