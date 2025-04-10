using FluentAssertions;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Domain.UnitTests.Primitives;

public class TestValueObject : ValueObject
{
    public string Name { get; }
    public int Age { get; }
    public bool IsActive { get; }

    public TestValueObject(string name, int age, bool isActive)
    {
        Name = name;
        Age = age;
        IsActive = isActive;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Age;
        yield return IsActive;
    }
}

public class DifferentTestValueObject : ValueObject
{
    public string Name { get; }
    public int Age { get; }

    public DifferentTestValueObject(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Age;
    }
}

public class ValueObjectTests
{
    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new TestValueObject("John", 30, true);

        // Act
        var result = valueObject1.Equals(valueObject2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new TestValueObject("John", 30, false);

        // Act
        var result = valueObject1.Equals(valueObject2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNullValueObject_ShouldReturnFalse()
    {
        // Arrange
        var valueObject = new TestValueObject("John", 30, true);
        TestValueObject? nullValueObject = null;

        // Act
        var result = valueObject.Equals(nullValueObject);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new DifferentTestValueObject("John", 30);

        // Act
        var result = valueObject1.Equals(valueObject2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNonValueObject_ShouldReturnFalse()
    {
        // Arrange
        var valueObject = new TestValueObject("John", 30, true);
        var nonValueObject = new object();

        // Act
        var result = valueObject.Equals(nonValueObject);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new TestValueObject("John", 30, true);

        // Act
        var hashCode1 = valueObject1.GetHashCode();
        var hashCode2 = valueObject2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new TestValueObject("John", 30, false);

        // Act
        var hashCode1 = valueObject1.GetHashCode();
        var hashCode2 = valueObject2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentTypes_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new DifferentTestValueObject("John", 30);

        // Act
        var hashCode1 = valueObject1.GetHashCode();
        var hashCode2 = valueObject2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void EqualsOperator_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new TestValueObject("John", 30, true);

        // Act
        var result = valueObject1 == valueObject2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void NotEqualsOperator_WithDifferentValues_ShouldReturnTrue()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        var valueObject2 = new TestValueObject("John", 30, false);

        // Act
        var result = valueObject1 != valueObject2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_WithNullFirstOperand_ShouldReturnFalse()
    {
        // Arrange
        TestValueObject? valueObject1 = null;
        var valueObject2 = new TestValueObject("John", 30, true);

        // Act
        var result = valueObject1 == valueObject2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_WithNullSecondOperand_ShouldReturnFalse()
    {
        // Arrange
        var valueObject1 = new TestValueObject("John", 30, true);
        TestValueObject? valueObject2 = null;

        // Act
        var result = valueObject1 == valueObject2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_WithBothOperandsNull_ShouldReturnTrue()
    {
        // Arrange
        TestValueObject? valueObject1 = null;
        TestValueObject? valueObject2 = null;

        // Act
        var result = valueObject1 == valueObject2;

        // Assert
        result.Should().BeTrue();
    }
}
