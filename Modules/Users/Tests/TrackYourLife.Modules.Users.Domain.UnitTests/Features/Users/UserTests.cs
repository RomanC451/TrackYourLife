using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Features.Users;

public class UserTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateUser()
    {
        // Arrange
        var id = UserId.NewId();
        var email = Email.Create("test@example.com").Value;
        var password = new HashedPassword("password123");
        var firstName = Name.Create("John").Value;
        var lastName = Name.Create("Doe").Value;

        // Act
        var result = User.Create(id, email, password, firstName, lastName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.Email.Should().Be(email);
        result.Value.PasswordHash.Should().Be(password);
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.Value.ModifiedOnUtc.Should().BeNull();
        result.Value.VerifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void ChangeName_WithDifferentNames_ShouldUpdateNamesAndModifiedOnUtc()
    {
        // Arrange
        var user = UserFaker.Generate();
        var newFirstName = Name.Create("NewFirstName").Value;
        var newLastName = Name.Create("NewLastName").Value;

        // Act
        user.ChangeName(newFirstName, newLastName);

        // Assert
        user.FirstName.Should().Be(newFirstName);
        user.LastName.Should().Be(newLastName);
        user.ModifiedOnUtc.Should().NotBeNull();
        user.ModifiedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ChangeName_WithSameNames_ShouldNotUpdateModifiedOnUtc()
    {
        // Arrange
        var user = UserFaker.Generate();
        var originalModifiedOnUtc = user.ModifiedOnUtc;

        // Act
        user.ChangeName(user.FirstName, user.LastName);

        // Assert
        user.ModifiedOnUtc.Should().Be(originalModifiedOnUtc);
    }

    [Fact]
    public void ChangeEmail_WithDifferentEmail_ShouldUpdateEmailAndModifiedOnUtc()
    {
        // Arrange
        var user = UserFaker.Generate();
        var newEmail = Email.Create("new@example.com").Value;

        // Act
        user.ChangeEmail(newEmail);

        // Assert
        user.Email.Should().Be(newEmail);
        user.ModifiedOnUtc.Should().NotBeNull();
        user.ModifiedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ChangeEmail_WithSameEmail_ShouldNotUpdateModifiedOnUtc()
    {
        // Arrange
        var user = UserFaker.Generate();
        var originalModifiedOnUtc = user.ModifiedOnUtc;

        // Act
        user.ChangeEmail(user.Email);

        // Assert
        user.ModifiedOnUtc.Should().Be(originalModifiedOnUtc);
    }

    [Fact]
    public void ChangePassword_WithDifferentPassword_ShouldUpdatePasswordAndModifiedOnUtc()
    {
        // Arrange
        var user = UserFaker.Generate();
        var newPassword = new HashedPassword("newpassword123");

        // Act
        user.ChangePassword(newPassword);

        // Assert
        user.PasswordHash.Should().Be(newPassword);
        user.ModifiedOnUtc.Should().NotBeNull();
        user.ModifiedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ChangePassword_WithSamePassword_ShouldNotUpdateModifiedOnUtc()
    {
        // Arrange
        var user = UserFaker.Generate();
        var originalModifiedOnUtc = user.ModifiedOnUtc;

        // Act
        user.ChangePassword(user.PasswordHash);

        // Assert
        user.ModifiedOnUtc.Should().Be(originalModifiedOnUtc);
    }

    [Fact]
    public void VerifyEmail_ShouldSetVerifiedOnUtc()
    {
        // Arrange
        var user = UserFaker.Generate();

        // Act
        user.VerifyEmail();

        // Assert
        user.VerifiedOnUtc.Should().NotBeNull();
        user.VerifiedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
