//using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

//namespace TrackYourLife.Modules.Users.Application.Users.Commands.UploadUserProfileImage;

//public sealed class UploadUserProfileImageCommandHandler(
//    Client supabaseClient,
//    IUserIdentifierProvider userIdentifierProvider
//) : ICommandHandler<UploadUserProfileImageCommand>
//{
//    public async Task<Result> Handle(
//        UploadUserProfileImageCommand command,
//        CancellationToken cancellationToken
//    )
//    {
//        using var memoryStream = new MemoryStream();

//        await command.File.CopyToAsync(memoryStream, cancellationToken);

//        var fileName = $"user-{userIdentifierProvider.UserId.Value}{command.File.GetExtension()}";

//        var list = await supabaseClient
//            .Storage.From(SupaBaseStorageBuckets.UsersProfilesImages)
//            .List();

//        if (list is not null && list.Find(file => file.Name == fileName) is not null)
//        {
//            await supabaseClient
//                .Storage.From(SupaBaseStorageBuckets.UsersProfilesImages)
//                .Update(
//                    memoryStream.ToArray(),
//                    fileName,
//                    new Supabase.Storage.FileOptions()
//                    {
//                        Upsert = true,
//                        ContentType = command.File.ContentType
//                    }
//                );
//            return Result.Success();
//        }

//        await supabaseClient
//            .Storage.From(SupaBaseStorageBuckets.UsersProfilesImages)
//            .Upload(memoryStream.ToArray(), fileName);

//        return Result.Success();
//    }
//}
