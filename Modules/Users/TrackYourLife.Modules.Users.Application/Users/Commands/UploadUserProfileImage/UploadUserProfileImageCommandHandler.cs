using Microsoft.IdentityModel.Tokens;
using Supabase;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Application.Core.Utils;
using TrackYourLife.Common.Contracts.Common;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.UploadUserProfileImage;

public sealed class UploadUserProfileImageCommandHandler(Client supabaseClient, IUserIdentifierProvider userIdentifierProvider)
        : ICommandHandler<UploadUserProfileImageCommand>
{

    private readonly Client _supabaseClient = supabaseClient;

    private readonly IUserIdentifierProvider _userIdentifierProvider = userIdentifierProvider;

    public async Task<Result> Handle(
        UploadUserProfileImageCommand command,
        CancellationToken cancellationToken
    )
    {

        using var memoryStream = new MemoryStream();

        await command.File.CopyToAsync(memoryStream, cancellationToken);

        var fileName = $"user-{_userIdentifierProvider.UserId.Value}{command.File.GetExtension()}";

        var list = await _supabaseClient.Storage.From(SupaBaseStorageBuckets.UsersProfilesImages).List();

        if (list is not null && list.Find(file => file.Name == fileName) is not null)
        {
            await _supabaseClient.Storage.From(SupaBaseStorageBuckets.UsersProfilesImages)
            .Update(memoryStream.ToArray(), fileName, new Supabase.Storage.FileOptions()
            {
                Upsert = true,
                ContentType = command.File.ContentType
            });
            return Result.Success();
        }

        await _supabaseClient.Storage.From(SupaBaseStorageBuckets.UsersProfilesImages)
            .Upload(memoryStream.ToArray(), fileName);

        return Result.Success();
    }


}
