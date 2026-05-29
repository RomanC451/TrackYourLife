using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.VerifyYoutubeSettingsPassword;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Commands;

internal sealed class VerifyYoutubeSettingsPassword(ISender sender)
    : Endpoint<VerifyYoutubeSettingsPasswordRequest, IResult>
{
    public override void Configure()
    {
        Post("password/verify");
        Group<YoutubeSettingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        VerifyYoutubeSettingsPasswordRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new VerifyYoutubeSettingsPasswordCommand(req.Password))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
