using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.TogglePlanTypeForDevelopment;

public sealed class TogglePlanTypeForDevelopmentCommandValidator
    : AbstractValidator<TogglePlanTypeForDevelopmentCommand>
{
    public TogglePlanTypeForDevelopmentCommandValidator()
    {
        // No validation rules needed as this is a parameterless command
    }
}
