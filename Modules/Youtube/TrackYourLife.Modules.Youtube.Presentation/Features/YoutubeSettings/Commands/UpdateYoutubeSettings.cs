using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;
using TrackYourLife.Modules.Youtube.Presentation.Contracts;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Commands;

internal sealed class UpdateYoutubeSettings(ISender sender)
    : Endpoint<UpdateYoutubeSettingsRequest, IResult>
{
    public override void Configure()
    {
        Put("");
        Group<YoutubeSettingsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateYoutubeSettingsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new UpdateYoutubeSettingsCommand(
                    MaxDivertissmentVideosPerDay: req.MaxDivertissmentVideosPerDay,
                    SettingsChangeFrequency: req.SettingsChangeFrequency,
                    DaysBetweenChanges: req.DaysBetweenChanges,
                    SpecificDayOfWeek: req.SpecificDayOfWeek,
                    SpecificDayOfMonth: req.SpecificDayOfMonth
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(settingsId => $"{ApiRoutes.Settings}/{settingsId.Value}");
    }
}
