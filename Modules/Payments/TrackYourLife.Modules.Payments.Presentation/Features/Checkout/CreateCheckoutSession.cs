using System.Net.Mime;
using TrackYourLife.Modules.Payments.Application.Features.Checkout.CreateCheckoutSession;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.Extensions;

namespace TrackYourLife.Modules.Payments.Presentation.Features.Checkout;

internal sealed record CreateCheckoutSessionRequest(
    string SuccessUrl,
    string CancelUrl,
    string PriceId
);

internal sealed class CreateCheckoutSession(ISender sender)
    : Endpoint<CreateCheckoutSessionRequest, IResult>
{
    public override void Configure()
    {
        Post("checkout-session");
        Group<PaymentsGroup>();
        Description(x =>
            x.Produces<string>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        CreateCheckoutSessionRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new CreateCheckoutSessionCommand(req.SuccessUrl, req.CancelUrl, req.PriceId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
