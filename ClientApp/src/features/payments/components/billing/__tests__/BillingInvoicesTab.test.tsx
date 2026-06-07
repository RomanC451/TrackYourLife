import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { invoice } from "@/features/payments/__tests__/fixtures";

vi.mock("../InvoiceHistoryCard", () => ({
  InvoiceHistoryCard: ({ invoices }: { invoices: unknown[] }) => (
    <div data-testid="invoice-history" data-count={invoices.length} />
  ),
}));

import { BillingInvoicesTab } from "../BillingInvoicesTab";

describe("BillingInvoicesTab", () => {
  it("delegates to InvoiceHistoryCard", () => {
    render(<BillingInvoicesTab invoices={[invoice(), invoice({ id: "inv-2" })]} />);

    expect(screen.getByTestId("invoice-history")).toHaveAttribute("data-count", "2");
  });
});
