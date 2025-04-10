using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Commands.UpdateFoodApiCookies;

internal sealed class UpdateFoodApiCookiesCommandHandler(
    IFoodApiCookiesManager foodApiCookiesManager,
    FoodApiCookieContainer foodApiCookieContainer
) : ICommandHandler<UpdateFoodApiCookiesCommand>
{
    public async Task<Result> Handle(
        UpdateFoodApiCookiesCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await foodApiCookiesManager.AddCookiesFromFilesToDbAsync(
            command.CookieFile,
            cancellationToken
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        foreach (var cookie in result.Value)
        {
            foodApiCookieContainer.Add(cookie);
        }
        return Result.Success();
    }
}
