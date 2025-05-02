using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.DeleteCurrentUser;

public sealed class RemoveCurrentUserCommandValidator : AbstractValidator<RemoveCurrentUserCommand>
{
    public RemoveCurrentUserCommandValidator()
    {
        // No validation rules needed as this is a parameterless command
    }
}
