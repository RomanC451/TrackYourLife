import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { billingSummary } from "@/features/payments/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseSuspenseQuery } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("@/features/payments/components/billing/index", () => ({
  BillingOverviewTab: () => <div data-testid="billing-overview-tab" />,
  BillingInvoicesTab: () => <div data-testid="billing-invoices-tab" />,
  UsageTab: () => <div data-testid="billing-usage-tab" />,
}));

import BillingPage from "../BillingPage";

describe("BillingPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({ data: billingSummary() });
  });

  it("renders billing tabs and overview by default", () => {
    render(<BillingPage />, { wrapper: createQueryClientWrapper() });

    expect(screen.getByText("Billing")).toBeInTheDocument();
    expect(screen.getByTestId("billing-overview-tab")).toBeInTheDocument();
  });

  it("renders invoices and usage tabs", () => {
    render(<BillingPage />, { wrapper: createQueryClientWrapper() });

    expect(screen.getByRole("tab", { name: "Invoices" })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "Usage" })).toBeInTheDocument();
  });
});
