using System.Runtime.CompilerServices;
using MassTransit;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Commands.UpdateFoodApiCookies;

public class UpdateFoodApiCookiesCommandHandler(
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
