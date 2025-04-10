using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Infrastructure.Data;
using Xunit;

namespace TrackYourLife.SharedLib.Infrastructure.UnitTests.Data
{
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
    }

    public sealed class TestAuditableEntity : Entity<TestId>, IAuditableEntity
    {
        private TestAuditableEntity() { }

        public TestAuditableEntity(TestId id, string name)
            : base(id)
        {
            Name = name;
        }

        public string Name { get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }

    public sealed class TestAggregateRoot : AggregateRoot<TestId>
    {
        private TestAggregateRoot() { }

        public TestAggregateRoot(TestId id, string name)
            : base(id)
        {
            Name = name;
        }

        public string Name { get; set; } = string.Empty;

        public override void OnDelete()
        {
            RaiseDirectDomainEvent(new TestDomainEvent(Id));
        }
    }

    public sealed class TestDomainEvent : IDirectDomainEvent
    {
        public TestDomainEvent(TestId entityId)
        {
            EntityId = entityId;
        }

        public TestId EntityId { get; }
    }

    public sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }

        public DbSet<TestAuditableEntity> AuditableEntities { get; set; } = null!;
        public DbSet<TestAggregateRoot> AggregateRoots { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestAuditableEntity>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder
                    .Property(e => e.Id)
                    .HasConversion(id => id.Value, value => new TestId(value));
            });

            modelBuilder.Entity<TestAggregateRoot>(builder =>
            {
                builder.HasKey(e => e.Id);
                builder
                    .Property(e => e.Id)
                    .HasConversion(id => id.Value, value => new TestId(value));
            });
        }
    }

    public sealed class TestAuditableEntityRepository
        : GenericRepository<TestAuditableEntity, TestId>
    {
        public TestAuditableEntityRepository(DbSet<TestAuditableEntity> dbSet)
            : base(dbSet) { }
    }

    public sealed class TestAggregateRootRepository : GenericRepository<TestAggregateRoot, TestId>
    {
        public TestAggregateRootRepository(DbSet<TestAggregateRoot> dbSet)
            : base(dbSet) { }
    }

    public class UnitOfWorkTests : IDisposable
    {
        private readonly TestDbContext _context;
        private readonly UnitOfWork<TestDbContext> _unitOfWork;
        private readonly TestAuditableEntityRepository _auditableEntityRepository;
        private readonly TestAggregateRootRepository _aggregateRootRepository;
        private readonly string _databaseName = Guid.NewGuid().ToString();
        private bool _disposed;

        public UnitOfWorkTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: _databaseName)
                .Options;

            _context = new TestDbContext(options);
            _unitOfWork = new UnitOfWork<TestDbContext>(_context);
            _auditableEntityRepository = new TestAuditableEntityRepository(
                _context.AuditableEntities
            );
            _aggregateRootRepository = new TestAggregateRootRepository(_context.AggregateRoots);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task GetDomainEvents_WhenDeletingAggregateRoot_ShouldReturnDeleteEvent()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot(TestId.New(), "Test");
            await _aggregateRootRepository.AddAsync(aggregateRoot, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            _aggregateRootRepository.Remove(aggregateRoot);
            var events = _unitOfWork.GetDirectDomainEvents();

            // Assert
            events.Should().HaveCount(1);
            events.First().Should().BeOfType<TestDomainEvent>();
            ((TestDomainEvent)events.First()).EntityId.Should().Be(aggregateRoot.Id);
        }

        [Fact]
        public async Task SaveChangesAsync_WhenAddingAuditableEntity_ShouldSetCreatedOnUtc()
        {
            // Arrange
            var entity = new TestAuditableEntity(TestId.New(), "Test");
            await _auditableEntityRepository.AddAsync(entity, CancellationToken.None);

            // Act
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            // Assert
            entity.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            entity.ModifiedOnUtc.Should().BeNull();
        }

        [Fact]
        public async Task SaveChangesAsync_WhenModifyingAuditableEntity_ShouldSetModifiedOnUtc()
        {
            // Arrange
            var entity = new TestAuditableEntity(TestId.New(), "Test");
            await _auditableEntityRepository.AddAsync(entity, CancellationToken.None);
            await _context.SaveChangesAsync();

            // Act
            entity.Name = "Updated";
            _auditableEntityRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            // Assert
            entity.ModifiedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task BeginTransactionAsync_ShouldReturnTransaction()
        {
            // Act
            var transaction = await _unitOfWork.BeginTransactionAsync(CancellationToken.None);

            // Assert
            transaction.Should().NotBeNull();
            transaction.Should().BeAssignableTo<IDbContextTransaction>();
        }

        [Fact]
        public async Task ReloadUpdatedEntitiesAsync_ShouldReloadModifiedEntities()
        {
            // Arrange
            var entity = new TestAuditableEntity(TestId.New(), "Test");
            await _auditableEntityRepository.AddAsync(entity, CancellationToken.None);
            await _context.SaveChangesAsync();

            var sameEntity = await _auditableEntityRepository.GetByIdAsync(
                entity.Id,
                CancellationToken.None
            );
            sameEntity!.Name = "Updated";
            _auditableEntityRepository.Update(sameEntity);
            await _context.SaveChangesAsync();

            // Act
            await _unitOfWork.ReloadUpdatedEntitiesAsync(CancellationToken.None);

            // Assert
            var reloadedEntity = _context.Entry(entity);
            reloadedEntity.State.Should().Be(EntityState.Unchanged);
            reloadedEntity.Entity.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task ReloadUpdatedEntitiesAsync_ShouldDetachAddedEntities()
        {
            // Arrange
            var entity = new TestAuditableEntity(TestId.New(), "Test");
            await _auditableEntityRepository.AddAsync(entity, CancellationToken.None);

            // Act
            await _unitOfWork.ReloadUpdatedEntitiesAsync(CancellationToken.None);

            // Assert
            var entry = _context.Entry(entity);
            entry.State.Should().Be(EntityState.Detached);
        }

        [Fact]
        public async Task ReloadUpdatedEntitiesAsync_ShouldReloadDeletedEntities()
        {
            // Arrange
            var entity = new TestAggregateRoot(TestId.New(), "Test");
            await _aggregateRootRepository.AddAsync(entity, CancellationToken.None);
            await _context.SaveChangesAsync();

            _aggregateRootRepository.Remove(entity);
            var changedEntity = _context.Entry(entity);
            changedEntity.State.Should().Be(EntityState.Deleted);

            // Act
            await _unitOfWork.ReloadUpdatedEntitiesAsync(CancellationToken.None);

            // Assert
            var reloadedEntity = _context.Entry(entity);
            reloadedEntity.State.Should().Be(EntityState.Unchanged);
        }

        [Fact]
        public async Task RealoadUpdatedEntitiesAsync_ShouldClearDomainEvents()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot(TestId.New(), "Test");
            await _aggregateRootRepository.AddAsync(aggregateRoot, CancellationToken.None);
            await _context.SaveChangesAsync();

            _aggregateRootRepository.Remove(aggregateRoot);

            // Act
            await _unitOfWork.ReloadUpdatedEntitiesAsync(CancellationToken.None);

            // Assert
            var entity = _context.Entry(aggregateRoot);
            entity.State.Should().Be(EntityState.Unchanged);

            aggregateRoot.GetDirectDomainEvents().Should().BeEmpty();
        }

        [Fact]
        public async Task GetDomainEvents_ShouldClearEventsAfterRetrieving()
        {
            // Arrange
            var aggregateRoot = new TestAggregateRoot(TestId.New(), "Test");
            await _aggregateRootRepository.AddAsync(aggregateRoot, CancellationToken.None);
            await _context.SaveChangesAsync();
            _aggregateRootRepository.Remove(aggregateRoot);

            // Act
            var firstEvents = _unitOfWork.GetDirectDomainEvents();

            // Assert
            var entity = _context.Entry(aggregateRoot);
            entity.State.Should().Be(EntityState.Deleted);
            firstEvents.Should().HaveCount(1);
            entity.Entity.GetDirectDomainEvents().Should().BeEmpty();
        }
    }
}
