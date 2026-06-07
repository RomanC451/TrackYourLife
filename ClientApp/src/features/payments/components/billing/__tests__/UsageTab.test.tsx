import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/features/payments/components/BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { UsageTab } from "../UsageTab";

describe("UsageTab", () => {
  it("explains usage tracking and links to billing portal", () => {
    render(<UsageTab />);

    expect(screen.getByText("Usage")).toBeInTheDocument();
    expect(
      screen.getByText(/Usage metrics are not tracked in TrackYourLife yet/i),
    ).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Open billing portal/i }),
    ).toBeInTheDocument();
  });
});
