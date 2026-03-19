using TrackYourLife.Modules.Payments.Application.Contracts;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Payments.Application.Features.BillingSummary.GetBillingSummary;

public sealed record GetBillingSummaryQuery : IQuery<BillingSummaryDto>;

