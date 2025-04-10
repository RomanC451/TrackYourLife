using FluentAssertions;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Domain.UnitTests.Primitives;

public class TestAggregateRoot : AggregateRoot<AggregateTestId>
{
    public TestAggregateRoot(AggregateTestId id)
        : base(id) { }

    public TestAggregateRoot()
        : base() { }

    public void RaiseTestOutboxEvent(IOutboxDomainEvent domainEvent)
    {
        RaiseOutboxDomainEvent(domainEvent);
    }

    public void RaiseTestDirectEvent(IDirectDomainEvent domainEvent)
    {
        RaiseDirectDomainEvent(domainEvent);
    }
}

public class AggregateTestId : IStronglyTypedGuid
{
    public Guid Value { get; }

    public AggregateTestId(Guid value)
    {
        Value = value;
    }
}

public class TestOutboxDomainEvent : IOutboxDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string? Description { get; set; }
}

public class TestDirectDomainEvent : IDirectDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string? Description { get; set; }
}

public class AggregateRootTests
{
    [Fact]
    public void Create_WithValidId_ShouldSetId()
    {
        // Arrange
        var id = new AggregateTestId(Guid.NewGuid());

        // Act
        var aggregate = new TestAggregateRoot(id);

        // Assert
        aggregate.Id.Should().Be(id);
    }

    [Fact]
    public void Create_WithoutId_ShouldInitializeDefaultId()
    {
        // Act
        var aggregate = new TestAggregateRoot();

        // Assert
        aggregate.Id.Should().BeNull();
    }

    [Fact]
    public void RaiseOutboxDomainEvent_WithValidEvent_ShouldAddEvent()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var domainEvent = new TestOutboxDomainEvent();

        // Act
        aggregate.RaiseTestOutboxEvent(domainEvent);

        // Assert
        var events = aggregate.GetOutboxDomainEvents();
        events.Should().ContainSingle();
        events.First().Should().Be(domainEvent);
    }

    [Fact]
    public void RaiseDirectDomainEvent_WithValidEvent_ShouldAddEvent()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var domainEvent = new TestDirectDomainEvent();

        // Act
        aggregate.RaiseTestDirectEvent(domainEvent);

        // Assert
        var events = aggregate.GetDirectDomainEvents();
        events.Should().ContainSingle();
        events.First().Should().Be(domainEvent);
    }

    [Fact]
    public void RaiseOutboxDomainEvent_WithMultipleEvents_ShouldAddAllEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var event1 = new TestOutboxDomainEvent { Description = "First" };
        var event2 = new TestOutboxDomainEvent { Description = "Second" };
        var event3 = new TestOutboxDomainEvent { Description = "Third" };

        // Act
        aggregate.RaiseTestOutboxEvent(event1);
        aggregate.RaiseTestOutboxEvent(event2);
        aggregate.RaiseTestOutboxEvent(event3);

        // Assert
        var events = aggregate.GetOutboxDomainEvents();
        events.Should().HaveCount(3);
        events.Should().Contain(event1);
        events.Should().Contain(event2);
        events.Should().Contain(event3);
    }

    [Fact]
    public void RaiseDirectDomainEvent_WithMultipleEvents_ShouldAddAllEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var event1 = new TestDirectDomainEvent { Description = "First" };
        var event2 = new TestDirectDomainEvent { Description = "Second" };
        var event3 = new TestDirectDomainEvent { Description = "Third" };

        // Act
        aggregate.RaiseTestDirectEvent(event1);
        aggregate.RaiseTestDirectEvent(event2);
        aggregate.RaiseTestDirectEvent(event3);

        // Assert
        var events = aggregate.GetDirectDomainEvents();
        events.Should().HaveCount(3);
        events.Should().Contain(event1);
        events.Should().Contain(event2);
        events.Should().Contain(event3);
    }

    [Fact]
    public void ClearOutboxDomainEvents_WithExistingEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var domainEvent = new TestOutboxDomainEvent();
        aggregate.RaiseTestOutboxEvent(domainEvent);

        // Act
        aggregate.ClearOutboxDomainEvents();

        // Assert
        aggregate.GetOutboxDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ClearDirectDomainEvents_WithExistingEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var domainEvent = new TestDirectDomainEvent();
        aggregate.RaiseTestDirectEvent(domainEvent);

        // Act
        aggregate.ClearDirectDomainEvents();

        // Assert
        aggregate.GetDirectDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void GetOutboxDomainEvents_ShouldReturnCopyOfEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var domainEvent = new TestOutboxDomainEvent();
        aggregate.RaiseTestOutboxEvent(domainEvent);

        // Act
        var events = aggregate.GetOutboxDomainEvents();
        aggregate.ClearOutboxDomainEvents();

        // Assert
        events.Should().ContainSingle();
        events.First().Should().Be(domainEvent);
    }

    [Fact]
    public void GetDirectDomainEvents_ShouldReturnCopyOfEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var domainEvent = new TestDirectDomainEvent();
        aggregate.RaiseTestDirectEvent(domainEvent);

        // Act
        var events = aggregate.GetDirectDomainEvents();
        aggregate.ClearDirectDomainEvents();

        // Assert
        events.Should().ContainSingle();
        events.First().Should().Be(domainEvent);
    }

    [Fact]
    public void RaiseOutboxDomainEvent_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        IOutboxDomainEvent domainEvent = null!;

        // Act & Assert
        Action act = () => aggregate.RaiseTestOutboxEvent(domainEvent);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RaiseDirectDomainEvent_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        IDirectDomainEvent domainEvent = null!;

        // Act & Assert
        Action act = () => aggregate.RaiseTestDirectEvent(domainEvent);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void NewAggregate_ShouldHaveEmptyEventCollections()
    {
        // Act
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));

        // Assert
        aggregate.GetOutboxDomainEvents().Should().BeEmpty();
        aggregate.GetDirectDomainEvents().Should().BeEmpty();
    }

    [Fact]
    public void ClearEvents_ShouldNotAffectOtherEventType()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(new AggregateTestId(Guid.NewGuid()));
        var outboxEvent = new TestOutboxDomainEvent();
        var directEvent = new TestDirectDomainEvent();
        aggregate.RaiseTestOutboxEvent(outboxEvent);
        aggregate.RaiseTestDirectEvent(directEvent);

        // Act
        aggregate.ClearOutboxDomainEvents();

        // Assert
        aggregate.GetOutboxDomainEvents().Should().BeEmpty();
        aggregate.GetDirectDomainEvents().Should().ContainSingle();
        aggregate.GetDirectDomainEvents().First().Should().Be(directEvent);
    }
}
