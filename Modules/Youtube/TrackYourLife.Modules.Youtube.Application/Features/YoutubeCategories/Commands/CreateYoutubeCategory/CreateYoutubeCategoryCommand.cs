using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.CreateYoutubeCategory;

public sealed record CreateYoutubeCategoryCommand(string Name, int MaxVideosPerDay)
    : ICommand<YoutubeCategoryId>;
