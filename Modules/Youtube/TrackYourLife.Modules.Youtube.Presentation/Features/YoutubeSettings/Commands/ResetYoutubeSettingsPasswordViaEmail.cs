using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.ResetYoutubeSettingsPasswordViaEmail;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Commands;

internal sealed class ResetYoutubeSettingsPasswordViaEmail(ISender sender)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("password/reset-email");
        Group<YoutubeSettingsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status500InternalServerError)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await sender
            .Send(new ResetYoutubeSettingsPasswordViaEmailCommand(), ct)
            .ToActionResultAsync();
    }
}
