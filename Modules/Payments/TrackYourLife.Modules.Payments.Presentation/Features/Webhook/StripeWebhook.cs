using System.Text;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleStripeWebhook;

namespace TrackYourLife.Modules.Payments.Presentation.Features.Webhook;

internal sealed class StripeWebhook(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("webhook");
        Group<PaymentsGroup>();
        AllowAnonymous();
        Description(x => x.Produces(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        using var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8);
        var jsonPayload = await reader.ReadToEndAsync(ct);

        if (string.IsNullOrEmpty(jsonPayload))
        {
            return TypedResults.BadRequest();
        }

        if (
            !HttpContext.Request.Headers.TryGetValue("Stripe-Signature", out var signatureHeader)
            || string.IsNullOrEmpty(signatureHeader)
        )
        {
            return TypedResults.BadRequest();
        }

        var result = await sender.Send(
            new HandleStripeWebhookCommand(jsonPayload, signatureHeader!),
            ct
        );

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.BadRequest();
    }
}
