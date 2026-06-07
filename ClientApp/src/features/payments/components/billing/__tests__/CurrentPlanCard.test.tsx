import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { subscription } from "@/features/payments/__tests__/fixtures";

vi.mock("@/features/payments/components/BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { CurrentPlanCard } from "../CurrentPlanCard";

describe("CurrentPlanCard", () => {
  it("renders active subscription details", () => {
    render(<CurrentPlanCard subscription={subscription()} />);

    expect(screen.getByText("Pro")).toBeInTheDocument();
    expect(screen.getByText("Active")).toBeInTheDocument();
    expect(screen.getByText(/Next billing:/i)).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Manage Subscription/i }),
    ).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Change Plan/i })).toHaveAttribute(
      "href",
      "/upgrade",
    );
  });

  it("renders free plan fallback without subscription", () => {
    render(<CurrentPlanCard subscription={undefined} />);

    expect(screen.getByText("Free")).toBeInTheDocument();
    expect(screen.getByText(/No active subscription/i)).toBeInTheDocument();
    expect(screen.getByText(/Upgrade to Pro to unlock all features/i)).toBeInTheDocument();
  });

  it.each([
    ["trialing", "Trial"],
    ["past_due", "Past due"],
    ["unpaid", "Past due"],
    ["canceled", "canceled"],
  ] as const)("renders %s status badge as %s", (status, label) => {
    render(<CurrentPlanCard subscription={subscription({ status })} />);

    expect(screen.getByText(label)).toBeInTheDocument();
  });
});
