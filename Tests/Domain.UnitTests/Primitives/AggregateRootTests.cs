using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.UnitTests.Primitives;

public class TestAggregateRoot : AggregateRoot<Guid>
{
    public TestAggregateRoot(Guid id)
        : base(id) { }

    public TestAggregateRoot()
        : base() { }

    public void TestRaiseDomainEvent(IDomainEvent domainEvent)
    {
        RaiseDomainEvent(domainEvent);
    }
}

public class AggregateRootTests
{
    [Fact]
    public void AggregateRoot_WhenConstructedWithId_SetsId()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        // Act
        TestAggregateRoot aggregateRoot = new(id);

        // Assert
        Assert.Equal(id, aggregateRoot.Id);
    }

    [Fact]
    public void AggregateRoot_WhenConstructedWithoutId_SetsEmptyId()
    {
        // Act
        TestAggregateRoot aggregateRoot = new();

        // Assert
        Assert.Equal(Guid.Empty, aggregateRoot.Id);
    }

    [Fact]
    public void AggregateRoot_GetDomainEvents_ReturnsEmptyListByDefault()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new();

        // Act
        IReadOnlyCollection<IDomainEvent> domainEvents = aggregateRoot.GetDomainEvents();

        // Assert
        Assert.Empty(domainEvents);
    }

    [Fact]
    public void AggregateRoot_ClearDomainEvents_RemovesAllDomainEvents()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new();
        aggregateRoot.TestRaiseDomainEvent(new TestDomainEvent());
        aggregateRoot.TestRaiseDomainEvent(new TestDomainEvent());
        IReadOnlyCollection<IDomainEvent> initialDomainEvents = aggregateRoot.GetDomainEvents();

        // Act
        aggregateRoot.ClearDomainEvents();
        IReadOnlyCollection<IDomainEvent> clearedDomainEvents = aggregateRoot.GetDomainEvents();

        // Assert
        Assert.NotEmpty(initialDomainEvents);
        Assert.Empty(clearedDomainEvents);
    }

    [Fact]
    public void AggregateRoot_RaiseDomainEvent_AddsDomainEvent()
    {
        // Arrange
        TestAggregateRoot aggregateRoot = new();
        TestDomainEvent domainEvent = new();

        // Act
        aggregateRoot.TestRaiseDomainEvent(domainEvent);
        IReadOnlyCollection<IDomainEvent> domainEvents = aggregateRoot.GetDomainEvents();

        // Assert
        Assert.Single(domainEvents);
        Assert.Equal(domainEvent, domainEvents.First());
    }
}

public class TestDomainEvent : IDomainEvent
{
    public Guid Id { get; init; }

    public TestDomainEvent()
    {
        Id = Guid.NewGuid();
    }
}
