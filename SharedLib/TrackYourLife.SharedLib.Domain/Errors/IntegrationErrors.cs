namespace TrackYourLife.SharedLib.Domain.Errors;

public static class IntegrationErrors
{
    public static class MassTransit
    {
        public static readonly Error FailedRequest = new(
            "Integration.MassTransit.FailedRequest",
            "Failed to send a request to the MassTransit service.",
            500
        );
    }
}
