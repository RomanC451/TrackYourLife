using FluentAssertions;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Data.Users;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Data.Users;

public class UserQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private UserQuery _query = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _query = new UserQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUserReadModel()
    {
        // Arrange
        var user = UserFaker.Generate();
        await WriteDbContext.Users.AddAsync(user);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _query.GetByEmailAsync(user.Email, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email.Value);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentEmail = Email.Create("nonexistent@example.com").Value;

        try
        {
            // Act
            var result = await _query.GetByEmailAsync(nonExistentEmail, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
