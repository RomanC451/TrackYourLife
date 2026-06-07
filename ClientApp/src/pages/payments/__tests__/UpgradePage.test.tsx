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

vi.mock("@/features/payments/components/UpgradeToProButton", () => ({
  UpgradeToProButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { useAuthenticationContext } from "@/contexts/AuthenticationContextProvider";

import UpgradePage from "../UpgradePage";

describe("UpgradePage", () => {
  it("shows pro member message when already subscribed", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: true,
    } as ReturnType<typeof useAuthenticationContext>);

    render(<UpgradePage />);

    expect(screen.getByText(/already a Pro member/i)).toBeInTheDocument();
  });

  it("shows upgrade pricing for free users", () => {
    vi.mocked(useAuthenticationContext).mockReturnValue({
      isPro: false,
    } as ReturnType<typeof useAuthenticationContext>);

    render(<UpgradePage />);

    expect(screen.getByText(/Unlock Your Full Potential/i)).toBeInTheDocument();
    expect(screen.getByText(/Unlimited exercises and workouts/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Upgrade to Pro/i })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /billing page/i })).toHaveAttribute(
      "href",
      "/billing",
    );
  });
});
