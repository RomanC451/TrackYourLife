using System.ComponentModel.DataAnnotations;

namespace TrackYourLifeDotnet.Application.Users.Commands.ResendVerificationEmail;

public sealed record ResendEmailVerificationRequest([Required] string? Email);
