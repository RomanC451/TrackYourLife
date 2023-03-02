using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public record LoginUserCommand(string Email, string Password) : ICommand<LoginUserResponse>;
