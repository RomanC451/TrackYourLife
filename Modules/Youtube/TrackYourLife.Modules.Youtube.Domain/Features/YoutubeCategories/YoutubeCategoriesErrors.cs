using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

public static class YoutubeCategoriesErrors
{
    public static readonly Error NameRequired = new(
        "Youtube.Category.NameRequired",
        "Category name is required.",
        400
    );

    public static readonly Error NameTooLong = new(
        "Youtube.Category.NameTooLong",
        "Category name is too long.",
        400
    );

    public static readonly Error DuplicateName = new(
        "Youtube.Category.DuplicateName",
        "A category with this name already exists.",
        409
    );

    public static readonly Error NotFound = new(
        "Youtube.Category.NotFound",
        "Category was not found.",
        404
    );

    public static readonly Error ForbiddenForPlan = new(
        "Youtube.Category.ForbiddenForPlan",
        "This action requires Pro.",
        403
    );

    public static readonly Error CategoryLimitReached = new(
        "Youtube.CategoryWatchLimitReached",
        "Daily watch limit for this category has been reached.",
        403
    );

    public static readonly Error DeleteRequiresConfirmation = new(
        "Youtube.Category.DeleteRequiresConfirmation",
        "Deleting a category with channels requires explicit confirmation.",
        400
    );

    public static readonly Error CannotAssignChannelToCategory = new(
        "Youtube.Category.CannotAssignChannel",
        "Cannot subscribe channels to this category on your current plan. Remove extra categories or upgrade to Pro.",
        403
    );

    public static readonly Func<UserId, Error> NotFoundForUser = userId =>
        new("Youtube.Category.NotFoundForUser", $"No categories found for user {userId}.", 404);
}
