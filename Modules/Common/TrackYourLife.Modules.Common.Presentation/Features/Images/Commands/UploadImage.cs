using TrackYourLife.Modules.Common.Application.Features.Images.Commands.UploadImage;

namespace TrackYourLife.Modules.Common.Presentation.Features.Images.Commands;

internal sealed record UploadImageRequest(IFormFile Image);

internal sealed class UploadImage(ISender sender) : Endpoint<UploadImageRequest, IResult>
{
    public override void Configure()
    {
        Post("images");
        Group<ImagesGroup>();
        Description(x =>
            x.Produces<string>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
        AllowFileUploads();
    }

    public override async Task<IResult> ExecuteAsync(UploadImageRequest req, CancellationToken ct)
    {
        if (Files.Count == 0)
        {
            return TypedResults.NoContent();
        }

        var file = Files[0];

        var result = await Result
            .Create(new UploadImageCommand(file))
            .BindAsync(command => sender.Send(command, ct));

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        return result.ToActionResult();
    }
}
