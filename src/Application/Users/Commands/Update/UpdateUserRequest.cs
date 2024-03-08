using System.ComponentModel.DataAnnotations;

namespace TrackYourLifeDotnet.Application.Users.Commands.Update;

public sealed record UpdateUserRequest([Required] string? FirstName, [Required] string? LastName);
