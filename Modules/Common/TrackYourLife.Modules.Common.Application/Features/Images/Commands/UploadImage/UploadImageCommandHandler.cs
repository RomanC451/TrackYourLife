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
        // Normalize path separators for Supabase (always use forward slashes)
        path = path.Replace('\\', '/');

        var result = await supaBaseStorage.UploadFileAsync("images", request.Image, path);

        if (result.IsFailure)
        {
            return result;
        }

        // Add a small delay to allow Supabase to process the upload
        // This helps with eventual consistency issues
        await Task.Delay(100, cancellationToken);

        var signedUrlResult = await supaBaseStorage.CreateSignedUrlAsync(
            "images",
            path
        );

        return signedUrlResult;
    }
}
