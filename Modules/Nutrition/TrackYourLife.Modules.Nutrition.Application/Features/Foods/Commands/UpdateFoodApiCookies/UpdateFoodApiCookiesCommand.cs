using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Commands.UpdateFoodApiCookies;

public sealed record UpdateFoodApiCookiesCommand(IFormFile CookieFile) : ICommand;
