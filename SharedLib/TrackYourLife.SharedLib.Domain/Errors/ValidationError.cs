namespace TrackYourLife.SharedLib.Domain.Errors;

public sealed record ValidationError(string Name, string Message);
