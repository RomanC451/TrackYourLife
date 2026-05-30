using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetHomeRecommendation;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos.Queries;

internal sealed record HomeRecommendationResponse(YoutubeVideoPreview? Video);

internal sealed class GetHomeRecommendation(ISender sender)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("home-recommendation");
        Group<YoutubeVideosGroup>();
        Description(x =>
            x.Produces<HomeRecommendationResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetHomeRecommendationQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(video => new HomeRecommendationResponse(video));
    }
}
