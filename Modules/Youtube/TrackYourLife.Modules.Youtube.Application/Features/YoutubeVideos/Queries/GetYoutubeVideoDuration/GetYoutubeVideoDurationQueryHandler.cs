using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetYoutubeVideoDuration;

internal sealed class GetYoutubeVideoDurationQueryHandler(IYoutubeApiService youtubeApiService)
    : IQueryHandler<GetYoutubeVideoDurationQuery, YoutubeVideoDuration>
{
    public async Task<Result<YoutubeVideoDuration>> Handle(
        GetYoutubeVideoDurationQuery request,
        CancellationToken cancellationToken
    )
    {
        var duration = await youtubeApiService.GetVideoDurationSecondsAsync(
            request.VideoId,
            cancellationToken
        );

        if (duration.IsFailure)
        {
            return Result.Failure<YoutubeVideoDuration>(duration.Error);
        }

        return Result.Success(new YoutubeVideoDuration(duration.Value));
    }
}
