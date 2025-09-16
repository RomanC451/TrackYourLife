using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Repositories;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.UnitTests.Behaviors;

public class GenericUnitOfWorkBehaviorTests
{
    private class TestUnitOfWorkBehavior
        : GenericUnitOfWorkBehavior<IUnitOfWork, IOutboxMessageRepository, IRequest<Result>, Result>
    {
        public TestUnitOfWorkBehavior(
            IUnitOfWork unitOfWork,
            IPublisher publisher,
            IOutboxMessageRepository outboxMessageRepository
        )
            : base(unitOfWork, publisher, outboxMessageRepository) { }
    }

    private class TestCommand : IRequest<Result> { }

    private class TestQuery : IRequest<Result> { }

    private class TestDomainEvent : IDirectDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;
    private readonly IDbContextTransaction _transaction;
    private readonly TestUnitOfWorkBehavior _behavior;
    private readonly IOutboxMessageRepository _outboxMessageRepository;

    public GenericUnitOfWorkBehaviorTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _publisher = Substitute.For<IPublisher>();
        _transaction = Substitute.For<IDbContextTransaction>();
        _outboxMessageRepository = Substitute.For<IOutboxMessageRepository>();

        _unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(_transaction);

        _behavior = new TestUnitOfWorkBehavior(_unitOfWork, _publisher, _outboxMessageRepository);
    }

    [Fact]
    public async Task Handle_WithQuery_ShouldNotUseTransaction()
    {
        // Arrange
        var query = new TestQuery();
        var response = Result.Success();
        var next = new RequestHandlerDelegate<Result>(() => Task.FromResult(response));

        // Act
        var result = await _behavior.Handle(query, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
        await _transaction.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCommand_ShouldUseTransaction()
    {
        // Arrange
        var command = new TestCommand();
        var response = Result.Success();
        var next = new RequestHandlerDelegate<Result>(() => Task.FromResult(response));

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        await _transaction.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCommand_ShouldSaveChanges()
    {
        // Arrange
        var command = new TestCommand();
        var response = Result.Success();
        var next = new RequestHandlerDelegate<Result>(() => Task.FromResult(response));

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        await _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithCommand_ShouldPublishDomainEvents()
    {
        // Arrange
        var command = new TestCommand();
        var response = Result.Success();
        var next = new RequestHandlerDelegate<Result>(() => Task.FromResult(response));
        var domainEvent = new TestDomainEvent();
        var domainEvents = new List<IDirectDomainEvent> { domainEvent }.AsReadOnly();

        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _unitOfWork.GetDirectDomainEvents().Returns(domainEvents);

        // Act
        await _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await _publisher.Received(1).Publish(domainEvent, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithConcurrencyException_ShouldRetry()
    {
        // Arrange
        var command = new TestCommand();
        var response = Result.Success();
        var next = new RequestHandlerDelegate<Result>(() => Task.FromResult(response));

        var callCount = 0;
        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(x =>
            {
                callCount++;
                if (callCount <= 2)
                {
                    throw new DbUpdateConcurrencyException();
                }
                return Task.CompletedTask;
            });

        // Act
        await _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await _unitOfWork.Received(2).ReloadUpdatedEntitiesAsync(Arg.Any<CancellationToken>());
        await _transaction.Received(2).RollbackAsync(Arg.Any<CancellationToken>());
        await _transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithMaxRetriesExceeded_ShouldThrowException()
    {
        // Arrange
        var command = new TestCommand();
        var response = Result.Success();
        var next = new RequestHandlerDelegate<Result>(() => Task.FromResult(response));

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(x => throw new DbUpdateConcurrencyException());

        // Act
        var act = () => _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }

    [Fact]
    public async Task Handle_WithException_ShouldRollbackAndThrow()
    {
        // Arrange
        var command = new TestCommand();
        var next = new RequestHandlerDelegate<Result>(() => throw new Exception("Test exception"));

        _unitOfWork.BeginTransactionAsync(Arg.Any<CancellationToken>()).Returns(_transaction);

        // Act
        var act = () => _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
        await _transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
        await _transaction.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
}
