using TrackYourLife.Modules.Payments.Presentation.Contracts;

namespace TrackYourLife.Modules.Payments.Presentation.Features;

internal sealed class PaymentsGroup : Group
{
    public PaymentsGroup()
    {
        Configure(
            ApiRoutes.Payments,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
