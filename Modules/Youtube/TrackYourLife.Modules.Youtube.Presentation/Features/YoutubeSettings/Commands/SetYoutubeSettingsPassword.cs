using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.SetYoutubeSettingsPassword;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Commands;

internal sealed class SetYoutubeSettingsPassword(ISender sender)
    : Endpoint<SetYoutubeSettingsPasswordRequest, IResult>
{
    public override void Configure()
    {
        Put("password");
        Group<YoutubeSettingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SetYoutubeSettingsPasswordRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new SetYoutubeSettingsPasswordCommand(
                    CurrentPassword: req.CurrentPassword,
                    NewPassword: req.NewPassword
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
