using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.SharedLib.Infrastructure.UnitTests.Data.Repositories;

public sealed class TestEntity : Entity<TestId>
{
    private TestEntity() { }

    public TestEntity(TestId id, string name)
        : base(id)
    {
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
}

public sealed class TestId : IStronglyTypedGuid, IEquatable<TestId>
{
    private TestId() { }

    public TestId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; private set; }

    public static TestId New() => new(Guid.NewGuid());

    public bool Equals(TestId? other)
    {
        if (other is null)
            return false;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((TestId)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(TestId? left, TestId? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(TestId? left, TestId? right)
    {
        return !(left == right);
    }
}

public sealed class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options) { }

    public DbSet<TestEntity> TestEntities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasConversion(id => id.Value, value => new TestId(value));
        });
    }
}

public sealed class TestGenericRepository(DbSet<TestEntity> dbSet)
    : GenericRepository<TestEntity, TestId>(dbSet)
{
    public new async Task<TestEntity?> FirstOrDefaultAsync(
        Specification<TestEntity, TestId> specification,
        CancellationToken cancellationToken
    ) => await base.FirstOrDefaultAsync(specification, cancellationToken);

    public new async Task<List<TestEntity>> WhereAsync(
        Specification<TestEntity, TestId> specification,
        CancellationToken cancellationToken
    ) => await base.WhereAsync(specification, cancellationToken);
}

public sealed class GenericRepositoryTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly TestGenericRepository _repository;
    private readonly TestEntity _testEntity;

    public GenericRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _testEntity = new TestEntity(TestId.New(), "Test");
        _context.TestEntities.Add(_testEntity);
        _context.SaveChanges();

        _repository = new TestGenericRepository(_context.TestEntities);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ShouldReturnEntity()
    {
        // Act
        var result = await _repository.GetByIdAsync(_testEntity.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(_testEntity.Id);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(TestId.New(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AnyAsync_WithSpecification_WhenEntityExists_ShouldReturnTrue()
    {
        // Arrange
        var specification = new TestSpecification(x => x.Name == "Test");

        // Act
        var result = await _repository.AnyAsync(specification, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_WithSpecification_WhenEntityDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var specification = new TestSpecification(x => x.Name == "NonExistent");

        // Act
        var result = await _repository.AnyAsync(specification, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithSpecification_WhenEntityExists_ShouldReturnEntity()
    {
        // Arrange
        var specification = new TestSpecification(x => x.Name == "Test");

        // Act
        var result = await _repository.FirstOrDefaultAsync(specification, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(_testEntity.Id);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithSpecification_WhenEntityDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var specification = new TestSpecification(x => x.Name == "NonExistent");

        // Act
        var result = await _repository.FirstOrDefaultAsync(specification, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task WhereAsync_WithSpecification_ShouldReturnMatchingEntities()
    {
        // Arrange
        var specification = new TestSpecification(x => x.Name == "Test");

        // Act
        var result = await _repository.WhereAsync(specification, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(_testEntity.Id);
        result[0].Name.Should().Be("Test");
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var newEntity = new TestEntity(TestId.New(), "New");

        // Act
        await _repository.AddAsync(newEntity, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var result = await _context.TestEntities.FindAsync(newEntity.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("New");
    }

    [Fact]
    public void AddRange_ShouldAddEntities()
    {
        // Arrange
        var entities = new List<TestEntity> { new(TestId.New(), "One"), new(TestId.New(), "Two") };

        // Act
        _repository.AddRange(entities);
        _context.SaveChanges();

        // Assert
        var results = _context.TestEntities.Where(e => entities.Select(x => x.Id).Contains(e.Id));
        results.Should().HaveCount(2);
        results.Select(e => e.Name).Should().BeEquivalentTo("One", "Two");
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddEntities()
    {
        // Arrange
        var entities = new List<TestEntity> { new(TestId.New(), "One"), new(TestId.New(), "Two") };

        // Act
        await _repository.AddRangeAsync(entities, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var results = _context.TestEntities.Where(e => entities.Select(x => x.Id).Contains(e.Id));
        results.Should().HaveCount(2);
        results.Select(e => e.Name).Should().BeEquivalentTo("One", "Two");
    }

    [Fact]
    public void Update_ShouldUpdateEntity()
    {
        // Arrange
        var updatedEntity = new TestEntity(_testEntity.Id, "Updated");

        // Act
        _context.ChangeTracker.Clear(); // Clear the tracker before updating
        _repository.Update(updatedEntity);
        _context.SaveChanges();

        // Assert
        var result = _context.TestEntities.Find(_testEntity.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated");
    }

    [Fact]
    public void Remove_ShouldRemoveEntity()
    {
        // Act
        _repository.Remove(_testEntity);
        _context.SaveChanges();

        // Assert
        var result = _context.TestEntities.Find(_testEntity.Id);
        result.Should().BeNull();
    }
}

public sealed class TestSpecification(Expression<Func<TestEntity, bool>> expression)
    : Specification<TestEntity, TestId>
{
    private readonly Expression<Func<TestEntity, bool>> _expression = expression;

    public override Expression<Func<TestEntity, bool>> ToExpression() => _expression;
}
