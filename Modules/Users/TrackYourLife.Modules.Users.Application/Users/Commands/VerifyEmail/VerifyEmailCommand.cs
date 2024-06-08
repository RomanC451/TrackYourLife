using TrackYourLife.Common.Application.Core.Abstractions.Messaging;

namespace TrackYourLife.Modules.Users.Application.Users.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(string VerificationToken) : ICommand;
