using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

internal sealed record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
);

/// <summary>
/// Represents a class that handles the registration of a user.
/// </summary>
internal class RegisterUser(ISender sender, IUsersMapper mapper)
    : Endpoint<RegisterUserRequest, IResult>
{
    /// <summary>
    /// Configures the registration user endpoint.
    /// </summary>
    public override void Configure()
    {
        // Configure the HTTP method and route for the endpoint
        Post("register");

        // Group the endpoint under the AuthenticationGroup
        Group<AuthenticationGroup>();

        // Specifies the response types produced by the endpoint
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );

        // Allows anonymous access to the endpoint
        AllowAnonymous();
    }

    /// <summary>
    /// Executes the registration of a user asynchronously.
    /// </summary>
    /// <param name="req">The registration user request.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The result of the registration operation.</returns>
    public override async Task<IResult> ExecuteAsync(RegisterUserRequest req, CancellationToken ct)
    { // Create a result with the unprocessable request error
        var result = await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<RegisterUserCommand>)
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails())
        };
    }
}
