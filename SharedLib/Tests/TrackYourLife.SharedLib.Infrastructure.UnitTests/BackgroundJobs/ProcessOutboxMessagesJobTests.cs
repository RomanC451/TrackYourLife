using FluentAssertions;
using MediatR;
using Newtonsoft.Json;
using NSubstitute;
using Quartz;
using Serilog;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Repositories;
using TrackYourLife.SharedLib.Infrastructure.BackgroundJobs;
using Xunit;

namespace TrackYourLife.SharedLib.Infrastructure.UnitTests.BackgroundJobs;

public sealed class ProcessOutboxMessagesJobTests
{
    private readonly IPublisher _publisher;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IJobExecutionContext _jobExecutionContext;
    private readonly ProcessOutboxMessagesJob _sut;

    public ProcessOutboxMessagesJobTests()
    {
        _publisher = Substitute.For<IPublisher>();
        _outboxMessageRepository = Substitute.For<IOutboxMessageRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger>();
        _jobExecutionContext = Substitute.For<IJobExecutionContext>();
        _jobExecutionContext.CancellationToken.Returns(CancellationToken.None);

        _sut = new ProcessOutboxMessagesJob(
            _publisher,
            _outboxMessageRepository,
            _unitOfWork,
            _logger
        );
    }

    [Fact]
    public async Task Execute_WhenNoMessages_ShouldLogAndReturn()
    {
        // Arrange
        _outboxMessageRepository
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<OutboxMessage>());

        // Act
        await _sut.Execute(_jobExecutionContext);

        // Assert
        _logger
            .Received(1)
            .Debug(Arg.Is<string>(s => s.Contains("No unprocessed messages")), Arg.Any<DateTime>());
        await _publisher
            .DidNotReceive()
            .Publish(Arg.Any<IOutboxDomainEvent>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenMessagesExist_ShouldProcessAllMessages()
    {
        // Arrange
        var testEvent = new TestEvent { Id = Guid.NewGuid(), OccurredOnUtc = DateTime.UtcNow };
        var messages = new List<OutboxMessage>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = JsonConvert.SerializeObject(
                    testEvent,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                ),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Content = JsonConvert.SerializeObject(
                    testEvent,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                ),
            },
        };

        _outboxMessageRepository
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(messages);

        // Act
        await _sut.Execute(_jobExecutionContext);

        // Assert
        _logger
            .Received(1)
            .Information(
                Arg.Is<string>(s => s.Contains("Processing")),
                messages.Count,
                Arg.Any<DateTime>()
            );
        await _publisher
            .Received(2)
            .Publish(Arg.Any<IOutboxDomainEvent>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        messages.Should().AllSatisfy(m => m.ProcessedOnUtc.Should().NotBeNull());
    }

    [Fact]
    public async Task Execute_WhenMessageDeserializationFails_ShouldLogErrorAndContinue()
    {
        // Arrange
        var testEvent = new TestEvent { Id = Guid.NewGuid(), OccurredOnUtc = DateTime.UtcNow };
        var messages = new List<OutboxMessage>
        {
            new() { Id = Guid.NewGuid(), Content = "invalid json" },
            new()
            {
                Id = Guid.NewGuid(),
                Content = JsonConvert.SerializeObject(
                    testEvent,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                ),
            },
        };

        _outboxMessageRepository
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(messages);

        // Act
        await _sut.Execute(_jobExecutionContext);

        // Assert
        _logger
            .Received(1)
            .Error(
                Arg.Any<Exception>(),
                "Failed to process OutboxMessage with id: {Id}. Content: {Content}",
                messages[0].Id,
                messages[0].Content
            );
        await _publisher
            .Received(1)
            .Publish(Arg.Any<IOutboxDomainEvent>(), Arg.Any<CancellationToken>());
        messages[1].ProcessedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task Execute_WhenPublishingFails_ShouldLogErrorAndContinue()
    {
        // Arrange
        var testEvent = new TestEvent { Id = Guid.NewGuid(), OccurredOnUtc = DateTime.UtcNow };
        var messages = new List<OutboxMessage>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Content = JsonConvert.SerializeObject(
                    testEvent,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                ),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Content = JsonConvert.SerializeObject(
                    testEvent,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                ),
            },
        };

        _outboxMessageRepository
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns(messages);
        _publisher
            .Publish(Arg.Any<IOutboxDomainEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception("Publish failed")));

        // Act
        await _sut.Execute(_jobExecutionContext);

        // Assert
        _logger
            .Received(2)
            .Error(
                Arg.Any<Exception>(),
                Arg.Is<string>(s => s.Contains("Failed to process")),
                Arg.Any<Guid>(),
                Arg.Any<string>()
            );

        messages.Should().AllSatisfy(m => m.ProcessedOnUtc.Should().BeNull());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenUnexpectedError_ShouldLogAndRethrow()
    {
        // Arrange
        var exception = new Exception("Unexpected error");
        _outboxMessageRepository
            .GetUnprocessedMessagesAsync(Arg.Any<CancellationToken>())
            .Returns<Task<List<OutboxMessage>>>(x => throw exception);

        // Act & Assert
        var act = () => _sut.Execute(_jobExecutionContext);

        var thrownException = await act.Should().ThrowAsync<JobExecutionException>();
        thrownException.WithMessage("Failed to process outbox messages");
        thrownException
            .Which.InnerException.Should()
            .BeOfType<Exception>()
            .Which.Message.Should()
            .Be("Unexpected error");

        _logger
            .Received(1)
            .Error(Arg.Any<Exception>(), Arg.Is<string>(s => s.Contains("Failed to execute")));
    }

    private sealed class TestEvent : IOutboxDomainEvent
    {
        public Guid Id { get; set; }
        public DateTime OccurredOnUtc { get; set; }
    }
}
