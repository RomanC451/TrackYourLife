using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Domain.Errors;

public static class InfrastructureErrors
{
    public static class HttpContext
    {
        public static readonly Error NotExists =
            new("HttpContext.NotExists", "The HttpContext does not exist");
    }

    public static class FoodApiService
    {
        public static readonly Error FoodNotFound =
            new("FoodApiService.NotFound", "The food name was not found in the database.");

        public static readonly Error FailedToParseHtml =
            new("FoodApiService.FailedToParseHtml", "Failed to parse the html from the food api.");

        public static readonly Error FoodApiNotAuthenticated =
            new(
                "FoodApiService.ApiNotAuthenticated",
                "Failed to authenticate to the food api.",
                true
            );

        public static readonly Error FiledJsonDeserialization =
            new(
                "FoodApiService.FiledJsonDeserialization",
                "Failed to deserialize the api response."
            );
    }

    public static class EmailService
    {
        public static readonly Error FailedToSendEmail =
            new("Email.FailedToSendEmail", "Failed to send email.");
    }
}
