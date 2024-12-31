using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;

public sealed record ResendEmailVerificationCommand(string Email) : ICommand;
