using TrackYourLifeDotnet.Application.Abstractions.Messaging;

namespace TrackYourLifeDotnet.Application.Users.Commands.Update;

public sealed record UpdateUserCommand(Guid Id, string FirstName, string LastName)
    : ICommand<UpdateUserResponse>;
