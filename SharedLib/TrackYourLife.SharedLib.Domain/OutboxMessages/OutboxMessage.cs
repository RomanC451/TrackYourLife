namespace TrackYourLife.SharedLib.Domain.OutboxMessages;

public sealed class OutboxMessage
{
    public const int MaxRetryCount = 3;

    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public int RetryCount { get; set; }

    public bool IsDirectEvent { get; set; }

    public string? Error { get; set; }

    public DateTime OccurredOnUtc { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }
}
