using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.TogglePlanTypeForDevelopment;

namespace TrackYourLife.Modules.Users.Presentation.Features.Users.Commands;

internal sealed class TogglePlanTypeForDevelopment(ISender sender, IWebHostEnvironment environment)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("dev/toggle-plan-type");
        Group<UsersGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        if (!environment.IsDevelopment())
        {
            return TypedResults.NotFound();
        }

        return await Result
            .Create(new TogglePlanTypeForDevelopmentCommand())
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
