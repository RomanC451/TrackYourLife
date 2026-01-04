namespace TrackYourLife.SharedLib.Domain.OutboxMessages;

public class EventFailedException(string message) : Exception(message);

public class MessageConsumerFailedException(string message) : Exception(message);
