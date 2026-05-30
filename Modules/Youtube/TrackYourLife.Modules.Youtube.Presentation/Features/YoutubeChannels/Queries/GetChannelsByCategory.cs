using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Queries;

internal sealed record GetChannelsByCategoryRequest
{
    [QueryParam]
    public Guid? YoutubeCategoryId { get; init; }

    [QueryParam]
    public bool FavoritesOnly { get; init; }
}

internal sealed class GetChannelsByCategory(ISender sender)
    : Endpoint<GetChannelsByCategoryRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<YoutubeChannelsGroup>();
        Description(x =>
            x.Produces<IEnumerable<YoutubeChannelDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetChannelsByCategoryRequest req,
        CancellationToken ct
    )
    {
        YoutubeCategoryId? categoryId = req.YoutubeCategoryId is null
            ? null
            : YoutubeCategoryId.Create(req.YoutubeCategoryId.Value);

        return await Result
            .Create(
                new GetChannelsByCategoryQuery(
                    YoutubeCategoryId: categoryId,
                    FavoritesOnly: req.FavoritesOnly
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(channels => channels.Select(c => c.ToDto()));
    }
}
