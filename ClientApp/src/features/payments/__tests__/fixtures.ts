import type {
  BillingDetailsSummaryDto,
  BillingSummaryDto,
  InvoiceSummaryDto,
  PaymentMethodSummaryDto,
  SubscriptionSummaryDto,
} from "@/services/openapi";

const baseDto = { isLoading: false, isDeleting: false };

export function subscription(
  overrides: Partial<SubscriptionSummaryDto> = {},
): SubscriptionSummaryDto {
  return {
    ...baseDto,
    planName: "Pro",
    unitAmount: 999,
    currency: "USD",
    interval: "month",
    status: "active",
    currentPeriodEndUtc: "2026-07-01T00:00:00Z",
    cancelAtPeriodEnd: false,
    ...overrides,
  };
}

export function paymentMethod(
  overrides: Partial<PaymentMethodSummaryDto> = {},
): PaymentMethodSummaryDto {
  return {
    ...baseDto,
    brand: "visa",
    last4: "4242",
    expMonth: 3,
    expYear: 2028,
    billingName: "Jane Doe",
    isExpiringSoon: false,
    ...overrides,
  };
}

export function billingDetails(
  overrides: Partial<BillingDetailsSummaryDto> = {},
): BillingDetailsSummaryDto {
  return {
    ...baseDto,
    companyName: "Acme Inc",
    vatId: "VAT123",
    billingAddress: {
      isLoading: false,
      isDeleting: false,
      line1: "123 Main St",
      line2: "Suite 4",
      city: "Springfield",
      state: "IL",
      postalCode: "62701",
      country: "US",
    },
    ...overrides,
  };
}

export function invoice(
  overrides: Partial<InvoiceSummaryDto> = {},
): InvoiceSummaryDto {
  return {
    ...baseDto,
    id: "inv-1",
    createdUtc: "2026-06-01T00:00:00Z",
    amount: 999,
    currency: "USD",
    status: "paid",
    invoicePdf: "https://example.com/invoice.pdf",
    hostedInvoiceUrl: "https://example.com/invoice",
    ...overrides,
  };
}

export function billingSummary(
  overrides: Partial<BillingSummaryDto> = {},
): BillingSummaryDto {
  return {
    ...baseDto,
    subscription: subscription(),
    paymentMethod: paymentMethod(),
    billingDetails: billingDetails(),
    invoices: [invoice()],
    ...overrides,
  };
}
