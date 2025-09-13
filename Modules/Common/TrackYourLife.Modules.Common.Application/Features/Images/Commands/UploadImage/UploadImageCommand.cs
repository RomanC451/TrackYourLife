using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Common.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Common.Application.Features.Images.Commands.UploadImage;

public sealed record UploadImageCommand(IFormFile Image) : ICommand<string>;
