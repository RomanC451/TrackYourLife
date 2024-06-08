using Microsoft.AspNetCore.Http;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.UploadUserProfileImage;

public sealed record UploadUserProfileImageCommand(IFormFile File) : ICommand;
