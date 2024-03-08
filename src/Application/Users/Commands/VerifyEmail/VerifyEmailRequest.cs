using System.ComponentModel.DataAnnotations;

namespace TrackYourLifeDotnet.Application.Users.Commands.VerifyEmail;

public sealed record VerifyEmailRequest([Required] string? token);
