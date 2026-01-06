namespace TrackYourLife.SharedLib.Domain.Errors;

public static class InfrastructureErrors
{
    public static class HttpContext
    {
        public static readonly Error NotExists = new(
            "HttpContext.NotExists",
            "The HttpContext does not exist",
            500
        );
    }

    public static class FoodApiService
    {
        public static readonly Error FoodNotFound = new(
            "FoodApiService.NotFound",
            "The food name was not found in the database.",
            404
        );

        public static readonly Error FailedToParseHtml = new(
            "FoodApiService.FailedToParseHtml",
            "Failed to parse the html from the food api.",
            500
        );

        public static readonly Error FoodApiNotAuthenticated = new(
            "FoodApiService.ApiNotAuthenticated",
            "Failed to authenticate to the food api.",
            500
        );

        public static readonly Error FailedJsonDeserialization = new(
            "FoodApiService.FiledJsonDeserialization",
            "Failed to deserialize the api response.",
            500
        );
    }

    public static class EmailService
    {
        public static readonly Error FailedToSendEmail = new(
            "Email.FailedToSendEmail",
            "Failed to send email.",
            500
        );

        public static readonly Error EmailTemplateNotFound = new(
            "EmailService.EmailTemplateNotFound",
            "Email template file not found",
            500
        );
    }

    public static class SupaBaseClient
    {
        public static readonly Error FileExists = new(
            "SupaBaseClient.FileExists",
            "File with the same name already exists",
            500
        );

        public static readonly Error ClientNotWorking = new(
            "SupaBaseClient.ClientNotWorking",
            "The SupaBase client is not working.",
            500
        );

        public static readonly Error NoFilesInBucket = new(
            "SupaBaseClient.NoFilesInBucket",
            "There are no files in the bucket.",
            500
        );

        public static readonly Error FileNotFound = new(
            "SupaBaseClient.FileNotFound",
            "The file was not found in the bucket.",
            500
        );

        public static readonly Error FailedToCreateSignedUrl = new(
            "SupaBaseClient.FailedToCreateSignedUrl",
            "Failed to create a signed URL for the file.",
            500
        );

        public static readonly Error InvalidSignedUrl = new(
            "SupaBaseClient.InvalidSignedUrl",
            "The signed URL is invalid.",
            500
        );
    }

    public static class CookieReader
    {
        public static readonly Error FailedToGetKey = new(
            "CookieReader.FailedToGetKey",
            "Failed to get the key from the local state file.",
            500
        );

        public static readonly Error FailedToReadCookies = new(
            "CookieReader.FailedToReadCookies",
            "Failed to read cookies from the file.",
            500
        );
    }
}
