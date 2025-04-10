using FluentAssertions;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.SharedLib.Domain.UnitTests.Utils;

public class EnsureTests
{
    [Fact]
    public void IsTrue_WithTrueCondition_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var result = Ensure.IsTrue(true, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void IsTrue_WithFalseCondition_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var result = Ensure.IsTrue(false, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void IsFalse_WithFalseCondition_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var result = Ensure.IsFalse(false, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void IsFalse_WithTrueCondition_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;

        // Act
        var result = Ensure.IsFalse(true, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void IsInEnum_WithValidEnumValue_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = DayOfWeek.Monday;

        // Act
        var result = Ensure.IsInEnum(value, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void IsInEnum_WithInvalidEnumValue_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = (DayOfWeek)999; // Invalid enum value

        // Act
        var result = Ensure.IsInEnum(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotNull_WithNonNullValue_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = "test";

        // Act
        var result = Ensure.NotNull(value, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void NotNull_WithNullValue_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        string? value = null;

        // Act
        var result = Ensure.NotNull(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotEmptyList_WithNonEmptyList_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var list = new List<int> { 1, 2, 3 };

        // Act
        var result = Ensure.NotEmptyList(list, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void NotEmptyList_WithEmptyList_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var list = new List<int>();

        // Act
        var result = Ensure.NotEmptyList(list, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotEmptyList_WithNullList_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        List<int>? list = null;

        // Act
        var result = Ensure.NotEmptyList(list!, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotEmpty_WithNonEmptyString_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = "test";

        // Act
        var result = Ensure.NotEmpty(value, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void NotEmpty_WithEmptyString_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = string.Empty;

        // Act
        var result = Ensure.NotEmpty(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotEmpty_WithWhitespaceString_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = "   ";

        // Act
        var result = Ensure.NotEmpty(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotEmpty_WithNullString_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        string? value = null;

        // Act
        var result = Ensure.NotEmpty(value!, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotEmpty_WithNonEmptyValue_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = 42;

        // Act
        var result = Ensure.NotEmpty(value, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void NotEmpty_WithDefaultValue_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = default(int);

        // Act
        var result = Ensure.NotEmpty(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotEmptyId_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var id = new TestId(Guid.NewGuid());

        // Act
        var result = Ensure.NotEmptyId(id, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void NotEmptyId_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var id = new TestId(Guid.Empty);

        // Act
        var result = Ensure.NotEmptyId(id, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void NotNegative_WithPositiveValue_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = 42.5f;

        // Act
        var result = Ensure.NotNegative(value, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void NotNegative_WithZeroValue_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = 0f;

        // Act
        var result = Ensure.NotNegative(value, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void NotNegative_WithNegativeValue_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = -42.5f;

        // Act
        var result = Ensure.NotNegative(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Positive_WithPositiveValue_ShouldReturnSuccess()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = 42.5f;

        // Act
        var result = Ensure.Positive(value, error);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Positive_WithZeroValue_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = 0f;

        // Act
        var result = Ensure.Positive(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Positive_WithNegativeValue_ShouldReturnFailure()
    {
        // Arrange
        var error = DomainErrors.General.UnProcessableRequest;
        var value = -42.5f;

        // Act
        var result = Ensure.Positive(value, error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    private class TestId : IStronglyTypedGuid
    {
        public TestId(Guid value)
        {
            Value = value;
        }

        public Guid Value { get; }
    }
}
