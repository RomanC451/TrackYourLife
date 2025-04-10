using FluentAssertions;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Domain.UnitTests.Primitives;

public class TestEntity : Entity<TestId>
{
    public TestEntity(TestId id)
        : base(id) { }

    public TestEntity()
        : base() { }
}

public class TestId : IStronglyTypedGuid
{
    public Guid Value { get; }

    public TestId(Guid value)
    {
        Value = value;
    }
}

public class EntityTests
{
    [Fact]
    public void Create_WithValidId_ShouldSetId()
    {
        // Arrange
        var id = new TestId(Guid.NewGuid());

        // Act
        var entity = new TestEntity(id);

        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Create_WithoutId_ShouldInitializeDefaultId()
    {
        // Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().BeNull();
    }

    [Fact]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var id = new TestId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(new TestId(Guid.NewGuid()));
        var entity2 = new TestEntity(new TestId(Guid.NewGuid()));

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNullEntity_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity(new TestId(Guid.NewGuid()));
        TestEntity? nullEntity = null;

        // Act
        var result = entity.Equals(nullEntity);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity(new TestId(Guid.NewGuid()));
        var differentEntity = new DifferentTestEntity(new TestId(Guid.NewGuid()));

        // Act
        var result = entity.Equals(differentEntity);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameId_ShouldReturnSameHashCode()
    {
        // Arrange
        var id = new TestId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var hashCode1 = entity1.GetHashCode();
        var hashCode2 = entity2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void EqualsOperator_WithSameId_ShouldReturnTrue()
    {
        // Arrange
        var id = new TestId(Guid.NewGuid());
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var result = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void NotEqualsOperator_WithDifferentId_ShouldReturnTrue()
    {
        // Arrange
        var entity1 = new TestEntity(new TestId(Guid.NewGuid()));
        var entity2 = new TestEntity(new TestId(Guid.NewGuid()));

        // Act
        var result = entity1 != entity2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void EqualsOperator_WithNullFirstOperand_ShouldReturnFalse()
    {
        // Arrange
        TestEntity? entity1 = null;
        var entity2 = new TestEntity(new TestId(Guid.NewGuid()));

        // Act
        var result = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_WithNullSecondOperand_ShouldReturnFalse()
    {
        // Arrange
        var entity1 = new TestEntity(new TestId(Guid.NewGuid()));
        TestEntity? entity2 = null;

        // Act
        var result = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_WithBothOperandsNull_ShouldReturnFalse()
    {
        // Arrange
        TestEntity? entity1 = null;
        TestEntity? entity2 = null;

        // Act
        var result = entity1 == entity2;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNonEntityObject_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity(new TestId(Guid.NewGuid()));
        var nonEntity = new object();

        // Act
        var result = entity.Equals(nonEntity);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithNullId_ShouldReturnZero()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        var hashCode = entity.GetHashCode();

        // Assert
        hashCode.Should().Be(0);
    }

    [Fact]
    public void Equals_WithBothIdsNull_ShouldReturnTrue()
    {
        // Arrange
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue();
    }
}

public class DifferentTestEntity : Entity<TestId>
{
    public DifferentTestEntity(TestId id)
        : base(id) { }
}
