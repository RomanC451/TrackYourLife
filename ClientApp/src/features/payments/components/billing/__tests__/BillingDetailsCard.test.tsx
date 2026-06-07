import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { billingDetails } from "@/features/payments/__tests__/fixtures";

vi.mock("@/features/payments/components/BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { BillingDetailsCard } from "../BillingDetailsCard";

describe("BillingDetailsCard", () => {
  it("renders billing address and tax details", () => {
    render(<BillingDetailsCard billingDetails={billingDetails()} />);

    expect(screen.getByText("123 Main St")).toBeInTheDocument();
    expect(screen.getByText("Suite 4")).toBeInTheDocument();
    expect(screen.getByText(/Springfield, IL, 62701/)).toBeInTheDocument();
    expect(screen.getByText("US")).toBeInTheDocument();
    expect(screen.getByText("Acme Inc")).toBeInTheDocument();
    expect(screen.getByText("VAT123")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Edit" })).toBeInTheDocument();
  });

  it("falls back to portal messaging when details are missing", () => {
    render(<BillingDetailsCard billingDetails={undefined} />);

    expect(screen.getAllByText(/Managed in Stripe billing portal/i)).toHaveLength(3);
  });
});
