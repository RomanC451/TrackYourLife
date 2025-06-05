namespace TrackYourLife.SharedLib.Domain.Errors;

public static class ErrorExtensions
{
    public static ValidationError ToValidationError(this Error error)
    {
        return new ValidationError(error.Code, error.Message);
    }
}
