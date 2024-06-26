using TrackYourLife.Common.Domain.Primitives;
using Xunit;

namespace TrackYourLife.Common.Domain.UnitTests.Primitives;

public class EntityTests
{
    [Fact]
    public void Entity_WithSameId_ShouldBeEqual()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity entity1 = new TestEntity(id);
        TestEntity entity2 = new TestEntity(id);

        // Act & Assert
        Assert.Equal(entity1, entity2);
        Assert.True(entity1 == entity2);
        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void Entity_WithDifferentId_ShouldNotBeEqual()
    {
        // Arrange
        TestEntity entity1 = new TestEntity(Guid.NewGuid());
        TestEntity entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(entity1, entity2);
        Assert.False(entity1 == entity2);
        Assert.True(entity1 != entity2);
    }

    [Fact]
    public void Entity_WithNull_ShouldNotBeEqual()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid());

        // Act & Assert
        Assert.NotNull(entity);
        Assert.False(entity == null);
        Assert.True(entity != null);
    }

    [Fact]
    public void Entity_WithDifferentType_ShouldNotBeEqual()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid());
        object obj = new();

        // Act & Assert
        Assert.NotEqual(entity, obj);
        Assert.False(entity.Equals(obj));
    }

    [Fact]
    public void Entity_WithSameId_ShouldHaveSameHashCode()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity entity1 = new TestEntity(id);
        TestEntity entity2 = new TestEntity(id);

        // Act & Assert
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void Entity_WithDifferentId_ShouldHaveDifferentHashCode()
    {
        // Arrange
        TestEntity entity1 = new TestEntity(Guid.NewGuid());
        TestEntity entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
    }

    private class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id)
            : base(id) { }
    }
}
