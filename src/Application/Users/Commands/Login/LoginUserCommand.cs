using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<LoginUserResult>;
