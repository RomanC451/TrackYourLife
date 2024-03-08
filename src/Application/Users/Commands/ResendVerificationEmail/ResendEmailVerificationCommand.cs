using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.ResendVerificationEmail;

public sealed record ResendEmailVerificationCommand(string Email) : ICommand;
