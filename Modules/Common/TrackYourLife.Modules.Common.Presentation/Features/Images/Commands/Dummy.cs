using TrackYourLife.Modules.Common.Application.Features.Images.Commands.UploadImage;

namespace TrackYourLife.Modules.Common.Presentation.Features.Images.Commands;


internal sealed class Dummy() : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("dummy");
        Group<ImagesGroup>();
        Description(x =>
            x.Produces<string>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Task.FromResult(Result.Failure(new Error("Dummy.Error", "Dummy error", 403)).ToActionResult());
    }
}
