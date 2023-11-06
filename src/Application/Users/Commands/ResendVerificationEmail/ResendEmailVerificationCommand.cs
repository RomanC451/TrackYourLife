using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.ResendVerificationEmail;

public record ResendEmailVerificationCommand(string Email) : ICommand; 

