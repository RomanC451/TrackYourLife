using TrackYourLife.Common.Domain.Primitives;

namespace TrackYourLife.Common.Domain.UnitTests.Primitives;

public class ValueObjectTests
{
    private class TestValueObject : ValueObject
    {
        public int Value1 { get; }
        public string Value2 { get; }

        public TestValueObject(int value1, string value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value1;
            yield return Value2;
        }
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenObjectsHaveSameValues()
    {
        // Arrange
        var obj1 = new TestValueObject(42, "foo");
        var obj2 = new TestValueObject(42, "foo");

        // Act
        var result = obj1.Equals(obj2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenObjectsHaveDifferentValues()
    {
        // Arrange
        var obj1 = new TestValueObject(42, "foo");
        var obj2 = new TestValueObject(23, "bar");

        // Act
        var result = obj1.Equals(obj2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashCode_WhenObjectsHaveSameValues()
    {
        // Arrange
        var obj1 = new TestValueObject(42, "foo");
        var obj2 = new TestValueObject(42, "foo");

        // Act
        var hash1 = obj1.GetHashCode();
        var hash2 = obj2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void GetHashCode_ReturnsDifferentHashCode_WhenObjectsHaveDifferentValues()
    {
        // Arrange
        var obj1 = new TestValueObject(42, "foo");
        var obj2 = new TestValueObject(23, "bar");

        // Act
        var hash1 = obj1.GetHashCode();
        var hash2 = obj2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }
}
