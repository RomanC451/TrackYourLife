using FluentValidation;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Must(name => Name.Create(name).IsSuccess)
            .WithMessage(x => Name.Create(x.FirstName).Error);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Must(name => Name.Create(name).IsSuccess)
            .WithMessage(x => Name.Create(x.LastName).Error);
    }
}
