using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UploadUserProfileImage;

internal sealed class UploadUserProfileImageCommandHandler(
    ISupaBaseStorage supabaseStorage,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UploadUserProfileImageCommand>
{
    public async Task<Result> Handle(
        UploadUserProfileImageCommand command,
        CancellationToken cancellationToken
    )
    {
        using var memoryStream = new MemoryStream();

        await command.File.CopyToAsync(memoryStream, cancellationToken);

        var fileName = $"user-{userIdentifierProvider.UserId.Value}{command.File.GetExtension()}";

        var result = await supabaseStorage.UploadFileAsync(
            SupaBaseStorageBuckets.UsersProfilesImages,
            command.File,
            fileName,
            false
        );

        return result;
    }
}
