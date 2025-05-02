using FluentValidation;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UploadUserProfileImage;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UploadUserProfileImage;

public sealed class UploadUserProfileImageCommandValidator
    : AbstractValidator<UploadUserProfileImageCommand>
{
    private const int MaxFileSizeInBytes = 5 * 1024 * 1024; // 5MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

    public UploadUserProfileImageCommandValidator()
    {
        RuleFor(x => x.File).NotNull();

        RuleFor(x => x.File).Must(file => file.Length > 0).WithMessage("File cannot be empty");

        RuleFor(x => x.File)
            .Must(file => file.Length <= MaxFileSizeInBytes)
            .WithMessage($"File size must be less than {MaxFileSizeInBytes / (1024 * 1024)}MB");

        RuleFor(x => x.File)
            .Must(file =>
                AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant())
            )
            .WithMessage(
                $"File must be one of the following types: {string.Join(", ", AllowedExtensions)}"
            );
    }
}
