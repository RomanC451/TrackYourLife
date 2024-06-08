using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Contracts.Users;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.LogIn;

public sealed record LogInUserCommand(string Email, string Password) : ICommand<TokenResponse>;
