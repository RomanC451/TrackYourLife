using TrackYourLife.Common.Application.Core.Abstractions.Messaging;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.ResendVerificationEmail;

public sealed record ResendEmailVerificationCommand(string Email) : ICommand;
