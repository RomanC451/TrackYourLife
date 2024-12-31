using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UploadUserProfileImage;

public sealed record UploadUserProfileImageCommand(IFormFile File) : ICommand;
