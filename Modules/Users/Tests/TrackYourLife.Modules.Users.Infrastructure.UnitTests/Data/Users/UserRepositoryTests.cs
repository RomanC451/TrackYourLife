using FluentAssertions;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Data.Users;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Data.Users;

[Collection("UsersRepositoryTests")]
public class UserRepositoryTests : BaseRepositoryTests
{
    private UserRepository _repository = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _repository = new UserRepository(_writeDbContext!);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = UserFaker.Generate();
        await _writeDbContext.Users.AddAsync(user);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.GetByEmailAsync(user.Email, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
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
            var result = await _repository.GetByEmailAsync(
                nonExistentEmail,
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
    public async Task IsEmailUniqueAsync_WhenEmailIsUnique_ReturnsTrue()
    {
        // Arrange
        var uniqueEmail = Email.Create("unique@example.com").Value;

        try
        {
            // Act
            var result = await _repository.IsEmailUniqueAsync(uniqueEmail, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task IsEmailUniqueAsync_WhenEmailExists_ReturnsFalse()
    {
        // Arrange
        var user = UserFaker.Generate();
        await _writeDbContext.Users.AddAsync(user);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _repository.IsEmailUniqueAsync(user.Email, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
