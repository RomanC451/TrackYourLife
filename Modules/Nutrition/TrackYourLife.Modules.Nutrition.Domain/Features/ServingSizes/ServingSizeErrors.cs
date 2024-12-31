using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

public static class ServingSizeErrors
{
    public static readonly Func<ServingSizeId, Error> NotFound = id =>
        Error.NotFound(id, arg2: nameof(ServingSize));
}
