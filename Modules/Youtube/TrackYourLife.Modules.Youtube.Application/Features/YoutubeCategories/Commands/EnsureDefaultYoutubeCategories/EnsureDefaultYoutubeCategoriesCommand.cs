using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.EnsureDefaultYoutubeCategories;

public sealed record EnsureDefaultYoutubeCategoriesCommand(UserId UserId) : ICommand;
