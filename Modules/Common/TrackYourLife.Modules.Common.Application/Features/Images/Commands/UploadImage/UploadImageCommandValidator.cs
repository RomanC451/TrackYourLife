using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Common.Application.Features.Images.Commands.UploadImage;

internal sealed class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    public UploadImageCommandValidator()
    {
        RuleFor(c => c.Image)
            .NotEmpty()
            .Must(x =>
                x.GetExtension() == ".jpg"
                || x.GetExtension() == ".png"
                || x.GetExtension() == ".jpeg"
                || x.GetExtension() == ".webp"
            )
            .WithMessage("Image must be a jpg, png, jpeg or webp file")
            .Must(x => x.Length <= 10 * 1024 * 1024)
            .WithMessage("Image must be less than 10MB");
    }
}
