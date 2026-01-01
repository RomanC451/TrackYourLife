using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;

public sealed record PlayVideoCommand(string VideoId) : ICommand<YoutubeVideoDetails>;
