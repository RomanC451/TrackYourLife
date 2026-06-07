import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/contexts/AuthenticationContextProvider", () => ({
  useAuthenticationContext: vi.fn(),
}));

vi.mock("../BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";

import { CancelSubscriptionSection } from "../CancelSubscriptionSection";

describe("CancelSubscriptionSection", () => {
  it("renders nothing for free users", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: false,
    } as ReturnType<typeof useAuthenticationContext>);

    const { container } = render(<CancelSubscriptionSection />);

    expect(container).toBeEmptyDOMElement();
  });

  it("shows cancellation guidance for pro users", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: true,
    } as ReturnType<typeof useAuthenticationContext>);

    render(<CancelSubscriptionSection />);

    expect(screen.getByText("Cancel subscription")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Open billing portal to cancel/i }),
    ).toBeInTheDocument();
  });
});
