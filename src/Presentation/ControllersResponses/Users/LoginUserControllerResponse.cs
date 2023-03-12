namespace TrackYourLifeDotnet.Presentation.ControllersResponses.Users;

public record LoginUserControllerResponse(Guid UserId, string JwtToken);
