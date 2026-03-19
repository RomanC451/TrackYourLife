using TrackYourLife.Modules.Payments.Application.Contracts;
using TrackYourLife.Modules.Payments.Application.Features.BillingSummary.GetBillingSummary;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.Extensions;

namespace TrackYourLife.Modules.Payments.Presentation.Features.BillingSummary;

internal sealed class GetBillingSummary(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("billing-summary");
        Group<PaymentsGroup>();

        Description(x =>
            x.Produces<BillingSummaryDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetBillingSummaryQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync();
    }
}

