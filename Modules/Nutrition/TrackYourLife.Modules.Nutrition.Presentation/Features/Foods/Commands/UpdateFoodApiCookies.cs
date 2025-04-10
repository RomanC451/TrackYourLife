using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Commands.UpdateFoodApiCookies;
using TrackYourLife.SharedLib.Presentation.Extensions;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Foods.Commands;

internal sealed record UpdateFoodApiCookiesRequest(IFormFile CookieFile);

internal sealed class UpdateFoodApiCookies(ISender sender)
    : Endpoint<UpdateFoodApiCookiesRequest, IResult>
{
    public override void Configure()
    {
        Put("food-api-cookies");
        AllowAnonymous();
        Group<FoodsGroup>();
        Description(x => x.Produces(200).ProducesProblemFE<ProblemDetails>(400));
        AllowFileUploads();
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateFoodApiCookiesRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new UpdateFoodApiCookiesCommand(req.CookieFile))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
