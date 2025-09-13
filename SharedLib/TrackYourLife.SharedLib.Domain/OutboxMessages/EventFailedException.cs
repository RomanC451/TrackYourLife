namespace TrackYourLife.SharedLib.Domain.OutboxMessages;

public class EventFailedException(string message) : Exception(message);
