using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string VerificationToken) : ICommand;
