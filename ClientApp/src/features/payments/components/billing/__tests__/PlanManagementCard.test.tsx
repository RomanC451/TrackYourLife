import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/features/payments/components/BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { PlanManagementCard } from "../PlanManagementCard";

describe("PlanManagementCard", () => {
  it("renders plan management actions", () => {
    render(<PlanManagementCard />);

    expect(screen.getByText("Plan Management")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Upgrade Plan/i })).toHaveAttribute(
      "href",
      "/upgrade",
    );
    expect(screen.getByRole("button", { name: /Downgrade Plan/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Apply Promo Code/i })).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Cancel Subscription/i }),
    ).toBeInTheDocument();
  });
});
