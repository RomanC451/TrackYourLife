using TrackYourLife.Modules.Common.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Application.Features.Images.Commands.UploadImage;

internal sealed class UploadImageCommandHandler(ISupaBaseStorage supaBaseStorage)
    : ICommandHandler<UploadImageCommand, string>
{
    public async Task<Result<string>> Handle(
        UploadImageCommand request,
        CancellationToken cancellationToken
    )
    {
        var extension = request.Image.GetExtension();

        var path = Path.Combine("tmp", $"{Guid.NewGuid()}{extension}");

        var result = await supaBaseStorage.UploadFileAsync("images", request.Image, path);

        if (result.IsFailure)
        {
            return result;
        }

        var signedUrlResult = await supaBaseStorage.CreateSignedUrlAsync(
            "images",
            path
        );

        return signedUrlResult;
    }
}
