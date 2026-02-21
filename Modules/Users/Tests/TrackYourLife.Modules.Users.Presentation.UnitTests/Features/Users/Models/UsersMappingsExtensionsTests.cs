using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Presentation.Features.Users;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Users.Models;

public class UsersMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithUserReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var userId = UserId.NewId();
        var userReadModel = new UserReadModel(
            Id: userId,
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@example.com",
            PasswordHash: "hashed-password",
            VerifiedOnUtc: DateTime.UtcNow,
            PlanType: PlanType.Free,
            StripeCustomerId: null,
            SubscriptionEndsAtUtc: null,
            SubscriptionStatus: null
        );

        // Act
        var dto = userReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(userId);
        dto.Email.Should().Be("john.doe@example.com");
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
    }

    [Fact]
    public void ToDto_WithUserReadModelWithNullVerifiedOnUtc_ShouldMapCorrectly()
    {
        // Arrange
        var userId = UserId.NewId();
        var userReadModel = new UserReadModel(
            Id: userId,
            FirstName: "Jane",
            LastName: "Smith",
            Email: "jane.smith@example.com",
            PasswordHash: "hashed-password",
            VerifiedOnUtc: null,
            PlanType: PlanType.Free,
            StripeCustomerId: null,
            SubscriptionEndsAtUtc: null,
            SubscriptionStatus: null
        );

        // Act
        var dto = userReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(userId);
        dto.Email.Should().Be("jane.smith@example.com");
        dto.FirstName.Should().Be("Jane");
        dto.LastName.Should().Be("Smith");
    }
}
