using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotNet.Domain.Errors;

public static class InfrastructureErrors
{
    public static class HttpContext
    {
        public static readonly Error NotExists =
            new("HttpContext.NotExists", "The HttpContext does not exist");
    }
}