﻿namespace TrackYourLife.Modules.Users.Domain.OutboxMessages;

public sealed class OutboxMessageConsumer
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
