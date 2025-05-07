using System.Net;
using System.Net.Http.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Presentation.Features.Users.Commands;
using TrackYourLife.SharedLib.FunctionalTests;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features;

[Collection("Users Integration Tests")]
public class UsersTests(UsersFunctionalTestWebAppFactory factory)
    : UsersBaseIntegrationTest(factory)
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task GetCurrentUser_WithValidUser_ShouldReturnUser()
    {
        var client = CreateUnauthorizedClient();

        var email = _faker.Internet.Email();
        var firstName = _faker.Person.FirstName;
        var lastName = _faker.Person.LastName;

        await client.RegisterAndLoginNewUserAsync(
            email: email,
            firstName: firstName,
            lastName: lastName
        );

        // Act
        var response = await client.GetAsync("/api/users/me");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<UserDto>(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
        result.FirstName.Should().Be(firstName);
        result.LastName.Should().Be(lastName);
    }

    [Fact]
    public async Task UpdateCurrentUser_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var request = new UpdateCurrentUserRequest(FirstName: "John", LastName: "Doe");

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/me", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify user was updated
        var updatedUser = await _usersWriteDbContext
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == _user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.FirstName.Value.Should().Be(request.FirstName);
        updatedUser.LastName.Value.Should().Be(request.LastName);
    }

    [Fact]
    public async Task DeleteCurrentUser_WithValidUser_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var user = await client.RegisterAndLoginNewUserAsync();
        // Act
        var response = await client.DeleteAsync("/api/users/me");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify user was deleted
        var deletedUser = await _usersWriteDbContext
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        deletedUser.Should().BeNull();
    }
}
