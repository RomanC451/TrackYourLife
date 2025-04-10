using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.SharedLib.Infrastructure.UnitTests.Data.Queries;

public sealed class TestReadModel : IReadModel<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options) { }

    public DbSet<TestReadModel> TestModels { get; set; } = null!;
}

public sealed class TestGenericQuery : GenericQuery<TestReadModel, int>
{
    public TestGenericQuery(IQueryable<TestReadModel> query)
        : base(query) { }

    public new async Task<IEnumerable<TestReadModel>> WhereAsync(
        Specification<TestReadModel, int> specification,
        CancellationToken cancellationToken
    ) => await Task.FromResult(query.Where(specification));

    public new async Task<IEnumerable<TestReadModel>> WhereAsync(
        Expression<Func<TestReadModel, bool>> expression,
        CancellationToken cancellationToken
    ) => await Task.FromResult(query.Where(expression));

    public new async Task<TestReadModel?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken
    ) =>
        await Task.FromResult(
            await query.FirstOrDefaultAsync(e => Equals(e.Id, id), cancellationToken)
        );

    public new async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) =>
        await Task.FromResult(await query.AnyAsync(e => Equals(e.Id, id), cancellationToken));
}

public sealed class GenericQueryTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly TestGenericQuery _genericQuery;
    private readonly TestReadModel _testModel;

    public GenericQueryTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _testModel = new TestReadModel { Id = 1, Name = "Test" };
        _context.TestModels.Add(_testModel);
        _context.SaveChanges();

        _genericQuery = new TestGenericQuery(_context.TestModels.AsQueryable());
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
        var result = await _genericQuery.GetByIdAsync(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _genericQuery.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityExists_ShouldReturnTrue()
    {
        // Act
        var result = await _genericQuery.ExistsAsync(1, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _genericQuery.ExistsAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task WhereAsync_WithSpecification_ShouldReturnMatchingEntities()
    {
        // Arrange
        var specification = new TestSpecification(x => x.Name == "Test");

        // Act
        var result = await _genericQuery.WhereAsync(specification, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Test");
    }

    [Fact]
    public async Task WhereAsync_WithExpression_ShouldReturnMatchingEntities()
    {
        // Arrange
        Expression<Func<TestReadModel, bool>> expression = x => x.Name == "Test";

        // Act
        var result = await _genericQuery.WhereAsync(expression, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(1);
        result.First().Name.Should().Be("Test");
    }
}

public sealed class TestSpecification : Specification<TestReadModel, int>
{
    private readonly Expression<Func<TestReadModel, bool>> _expression;

    public TestSpecification(Expression<Func<TestReadModel, bool>> expression)
    {
        _expression = expression;
    }

    public override Expression<Func<TestReadModel, bool>> ToExpression() => _expression;
}
