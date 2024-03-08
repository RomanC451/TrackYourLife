using System.ComponentModel.DataAnnotations;

namespace TrackYourLifeDotnet.Application.Users.Commands.Login;

public sealed record LoginUserRequest([Required] string? Email, [Required] string? Password);
