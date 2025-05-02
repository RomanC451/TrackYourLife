using FluentAssertions;
using TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryValidatorTests
{
    private readonly GetCurrentUserQueryValidator _validator;

    public GetCurrentUserQueryValidatorTests()
    {
        _validator = new GetCurrentUserQueryValidator();
    }

    [Fact]
    public void Validate_WithValidQuery_ReturnsSuccess()
    {
        // Arrange
        var query = new GetCurrentUserQuery();

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
