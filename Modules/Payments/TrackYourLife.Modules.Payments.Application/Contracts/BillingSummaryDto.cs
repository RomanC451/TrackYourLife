namespace TrackYourLife.Modules.Payments.Application.Contracts;

public sealed record BillingSummaryDto(
    SubscriptionSummaryDto? Subscription,
    PaymentMethodSummaryDto? PaymentMethod,
    BillingDetailsSummaryDto? BillingDetails,
    IReadOnlyList<InvoiceSummaryDto> Invoices
);

public sealed record BillingDetailsSummaryDto(
    BillingAddressDto? BillingAddress,
    string? CompanyName,
    string? VatId
);

public sealed record BillingAddressDto(
    string? Line1,
    string? Line2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country
);

public sealed record SubscriptionSummaryDto(
    string? PlanName,
    decimal? UnitAmount,
    string? Currency,
    string? Interval,
    string Status,
    DateTime? CurrentPeriodEndUtc,
    bool CancelAtPeriodEnd
);

public sealed record PaymentMethodSummaryDto(
    string Brand,
    string Last4,
    int ExpMonth,
    int ExpYear,
    string? BillingName,
    bool IsExpiringSoon
);

public sealed record InvoiceSummaryDto(
    string Id,
    DateTime CreatedUtc,
    decimal Amount,
    string Currency,
    string Status,
    string? HostedInvoiceUrl,
    string? InvoicePdf,
    string? ReceiptUrl
);
