namespace TrackYourLifeDotnet.Presentation.ControllersResponses.Users;

public record RegisterUserControllerResponse(Guid UserId, string JwtToken);
