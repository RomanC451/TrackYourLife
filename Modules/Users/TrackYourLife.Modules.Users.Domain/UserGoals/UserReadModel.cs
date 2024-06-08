namespace TrackYourLife.Modules.Users.Domain.UserGoals;

public sealed record UserReadModel(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PasswordHash
);
