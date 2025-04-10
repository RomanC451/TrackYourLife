using FluentAssertions;
using MediatR;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Application.UnitTests.Behaviors;

public class TestRequest : IRequest<Result> { }

public class RequestLoggingBehaviorTests
{
    private readonly TestLogger _logger;
    private readonly RequestLoggingBehavior<TestRequest, Result> _sut;

    public RequestLoggingBehaviorTests()
    {
        _logger = new TestLogger();
        _sut = new RequestLoggingBehavior<TestRequest, Result>(_logger);
    }

    [Fact]
    public async Task Handle_WithSuccessfulRequest_ShouldLogSuccess()
    {
        // Arrange
        var request = new TestRequest();
        var response = Result.Success();

        // Act
        await _sut.Handle(request, () => Task.FromResult(response), CancellationToken.None);

        // Assert
        _logger.LogEntries.Should().HaveCount(2);
        _logger.LogEntries[0].Level.Should().Be(LogEventLevel.Information);
        _logger.LogEntries[1].Level.Should().Be(LogEventLevel.Information);
        _logger.LogEntries[0].Message.Should().Contain("Processing requestt");
        _logger.LogEntries[1].Message.Should().Contain("Completed request");
    }

    [Fact]
    public async Task Handle_WithFailedRequest_ShouldLogError()
    {
        // Arrange
        var request = new TestRequest();
        var error = new Error("Test", "Test error");
        var response = Result.Failure(error);

        // Act
        await _sut.Handle(request, () => Task.FromResult(response), CancellationToken.None);

        // Assert
        _logger.LogEntries.Should().HaveCount(2);
        _logger.LogEntries[0].Level.Should().Be(LogEventLevel.Information);
        _logger.LogEntries[1].Level.Should().Be(LogEventLevel.Error);
        _logger.LogEntries[0].Message.Should().Contain("Processing requestt");
        _logger
            .LogEntries[1]
            .Message.Should()
            .Contain("Completed request")
            .And.Contain("with error");
    }

    [Fact]
    public async Task Handle_WithFailedRequest_ShouldPushErrorAsProperty()
    {
        // Arrange
        var request = new TestRequest();
        var error = new Error("Test", "Test error");
        var response = Result.Failure(error);

        // Act
        await _sut.Handle(request, () => Task.FromResult(response), CancellationToken.None);

        // Assert
        _logger.LogEntries.Should().HaveCount(2);
        var errorLogEntry = _logger.LogEntries[1];
        errorLogEntry.Level.Should().Be(LogEventLevel.Error);
        errorLogEntry
            .Message.Should()
            .Contain("Completed request")
            .And.Contain("with error")
            .And.Contain(error.ToString());
    }
}

public class TestLogger : ILogger
{
    public List<(LogEventLevel Level, string Message)> LogEntries { get; } = new();

    public void Write(LogEvent logEvent)
    {
        LogEntries.Add((logEvent.Level, logEvent.RenderMessage()));
    }

    public void Information(string messageTemplate, params object?[]? propertyValues)
    {
        var logEvent = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Information,
            null,
            new MessageTemplate(messageTemplate, new List<MessageTemplateToken>()),
            propertyValues
                ?.Select(p => new LogEventProperty(p?.ToString() ?? "", new ScalarValue(p)))
                .ToList() ?? new List<LogEventProperty>()
        );
        Write(logEvent);
    }

    public void Error(string messageTemplate, params object?[]? propertyValues)
    {
        var logEvent = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Error,
            null,
            new MessageTemplate(messageTemplate, new List<MessageTemplateToken>()),
            propertyValues
                ?.Select(p => new LogEventProperty(p?.ToString() ?? "", new ScalarValue(p)))
                .ToList() ?? new List<LogEventProperty>()
        );
        Write(logEvent);
    }

    public void Debug(string messageTemplate, params object?[]? propertyValues)
    {
        var logEvent = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Debug,
            null,
            new MessageTemplate(messageTemplate, new List<MessageTemplateToken>()),
            propertyValues
                ?.Select(p => new LogEventProperty(p?.ToString() ?? "", new ScalarValue(p)))
                .ToList() ?? new List<LogEventProperty>()
        );
        Write(logEvent);
    }

    public void Warning(string messageTemplate, params object?[]? propertyValues)
    {
        var logEvent = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Warning,
            null,
            new MessageTemplate(messageTemplate, new List<MessageTemplateToken>()),
            propertyValues
                ?.Select(p => new LogEventProperty(p?.ToString() ?? "", new ScalarValue(p)))
                .ToList() ?? new List<LogEventProperty>()
        );
        Write(logEvent);
    }

    public void Fatal(string messageTemplate, params object?[]? propertyValues)
    {
        var logEvent = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Fatal,
            null,
            new MessageTemplate(messageTemplate, new List<MessageTemplateToken>()),
            propertyValues
                ?.Select(p => new LogEventProperty(p?.ToString() ?? "", new ScalarValue(p)))
                .ToList() ?? new List<LogEventProperty>()
        );
        Write(logEvent);
    }

    public void Verbose(string messageTemplate, params object?[]? propertyValues)
    {
        var logEvent = new LogEvent(
            DateTimeOffset.Now,
            LogEventLevel.Verbose,
            null,
            new MessageTemplate(messageTemplate, new List<MessageTemplateToken>()),
            propertyValues
                ?.Select(p => new LogEventProperty(p?.ToString() ?? "", new ScalarValue(p)))
                .ToList() ?? new List<LogEventProperty>()
        );
        Write(logEvent);
    }

    public bool IsEnabled(LogEventLevel level)
    {
        return true;
    }

    public ILogger ForContext(ILogEventEnricher enricher)
    {
        return this;
    }

    public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
    {
        return this;
    }

    public ILogger ForContext(string propertyName, object? value, bool destructureObjects = false)
    {
        return this;
    }

    public ILogger ForContext<TSource>()
    {
        return this;
    }

    public ILogger ForContext(Type source)
    {
        return this;
    }
}
