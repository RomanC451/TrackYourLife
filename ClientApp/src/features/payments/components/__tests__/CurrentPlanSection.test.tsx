import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@/contexts/AuthenticationContextProvider", () => ({
  useAuthenticationContext: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, to }: { children: React.ReactNode; to: string }) => (
    <a href={to}>{children}</a>
  ),
}));

vi.mock("../BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";

import { CurrentPlanSection } from "../CurrentPlanSection";

describe("CurrentPlanSection", () => {
  it("shows upgrade prompt for free users", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: false,
      userData: undefined,
    } as ReturnType<typeof useAuthenticationContext>);

    render(<CurrentPlanSection />);

    expect(screen.getByText("Free")).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Upgrade to Pro/i })).toHaveAttribute(
      "href",
      "/upgrade",
    );
  });

  it("shows active subscription label without an end date", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: true,
      userData: {},
    } as ReturnType<typeof useAuthenticationContext>);

    render(<CurrentPlanSection />);

    expect(screen.getByText("Active subscription")).toBeInTheDocument();
  });

  it("shows active pro plan details", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: true,
      userData: {
        subscriptionEndsAtUtc: "2099-01-01T00:00:00Z",
        subscriptionCancelAtPeriodEnd: false,
      },
    } as ReturnType<typeof useAuthenticationContext>);

    render(<CurrentPlanSection />);

    expect(screen.getByText("Pro")).toBeInTheDocument();
    expect(screen.getByText(/Renews/i)).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Manage or cancel subscription/i }),
    ).toBeInTheDocument();
  });

  it("shows cancellation notice when renewal is canceled", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: true,
      userData: {
        subscriptionEndsAtUtc: "2099-01-01T00:00:00Z",
        subscriptionCancelAtPeriodEnd: true,
      },
    } as ReturnType<typeof useAuthenticationContext>);

    render(<CurrentPlanSection />);

    expect(screen.getByText(/subscription renewal has been canceled/i)).toBeInTheDocument();
    expect(screen.getAllByText(/Access until/i).length).toBeGreaterThan(0);
  });

  it("shows ended label for expired subscriptions", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: true,
      userData: {
        subscriptionEndsAtUtc: "2020-01-01T00:00:00Z",
        subscriptionCancelAtPeriodEnd: false,
      },
    } as ReturnType<typeof useAuthenticationContext>);

    render(<CurrentPlanSection />);

    expect(screen.getByText(/Ends/i)).toBeInTheDocument();
  });
});
