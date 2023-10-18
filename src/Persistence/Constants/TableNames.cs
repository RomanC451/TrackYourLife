namespace TrackYourLifeDotnet.Persistence.Constants;

internal static class TableNames
{
    internal const string Users = nameof(Users);

    internal const string UserTokens = nameof(UserTokens);

    internal const string Gatherings = nameof(Gatherings);

    internal const string Invitations = nameof(Invitations);

    internal const string Attendees = nameof(Attendees);

    internal const string OutboxMessages = nameof(OutboxMessages);

    internal const string OutboxMessageConsumers = nameof(OutboxMessageConsumers);
}
