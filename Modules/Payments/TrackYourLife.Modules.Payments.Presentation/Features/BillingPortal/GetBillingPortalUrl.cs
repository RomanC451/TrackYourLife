using TrackYourLife.Modules.Payments.Application.Features.BillingPortal.GetBillingPortalUrl;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.Extensions;

namespace TrackYourLife.Modules.Payments.Presentation.Features.BillingPortal;

internal sealed record GetBillingPortalUrlRequest(string ReturnUrl);

internal sealed class GetBillingPortalUrl(ISender sender)
    : Endpoint<GetBillingPortalUrlRequest, IResult>
{
    public override void Configure()
    {
        Get("portal");
        Group<PaymentsGroup>();

        Description(x =>
            x.Produces<string>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetBillingPortalUrlRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetBillingPortalUrlQuery(req.ReturnUrl))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync();
    }
}
