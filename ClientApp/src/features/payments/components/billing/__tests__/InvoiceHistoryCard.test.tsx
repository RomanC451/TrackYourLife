import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { invoice } from "@/features/payments/__tests__/fixtures";

vi.mock("@/features/payments/components/BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { InvoiceHistoryCard } from "../InvoiceHistoryCard";

describe("InvoiceHistoryCard", () => {
  it("shows empty state when there are no invoices", () => {
    render(<InvoiceHistoryCard invoices={[]} />);

    expect(screen.getByText(/No invoices found yet/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /View in Stripe/i })).toBeInTheDocument();
  });

  it("renders invoice rows with download and view actions", () => {
    render(<InvoiceHistoryCard invoices={[invoice()]} />);

    expect(screen.getByText("paid")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Download/i })).toHaveAttribute(
      "href",
      "https://example.com/invoice.pdf",
    );
    expect(screen.getByRole("link", { name: /View invoice/i })).toHaveAttribute(
      "href",
      "https://example.com/invoice",
    );
  });

  it("highlights open invoices and shows pay action", () => {
    render(
      <InvoiceHistoryCard
        invoices={[
          invoice({
            id: "inv-open",
            status: "open",
            invoicePdf: undefined,
            hostedInvoiceUrl: "https://example.com/pay",
          }),
        ]}
      />,
    );

    expect(screen.getByText("open")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Download/i })).toBeDisabled();
    expect(screen.getByRole("link", { name: /Pay invoice/i })).toHaveAttribute(
      "href",
      "https://example.com/pay",
    );
  });
});
