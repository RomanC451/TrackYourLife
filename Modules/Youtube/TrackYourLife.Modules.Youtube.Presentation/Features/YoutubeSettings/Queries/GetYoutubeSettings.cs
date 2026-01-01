using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Queries;

internal sealed class GetYoutubeSettings(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<YoutubeSettingsGroup>();
        Description(x =>
            x.Produces<YoutubeSettingsDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new GetYoutubeSettingsQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(settings => settings?.ToDto());
    }
}
