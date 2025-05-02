using FluentValidation;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => Email.Create(email).IsSuccess)
            .WithMessage(command => $"Invalid email: {Email.Create(command.Email).Error}");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(password => Password.Create(password).IsSuccess)
            .WithMessage(command => $"Invalid password: {Password.Create(command.Password).Error}");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Must(firstName => Name.Create(firstName).IsSuccess)
            .WithMessage(command => $"Invalid first name: {Name.Create(command.FirstName).Error}");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Must(lastName => Name.Create(lastName).IsSuccess)
            .WithMessage(command => $"Invalid last name: {Name.Create(command.LastName).Error}");
    }
}
