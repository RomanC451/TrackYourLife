import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const { mockMutate, mockUseCreateCheckoutSessionMutation } = vi.hoisted(() => ({
  mockMutate: vi.fn(),
  mockUseCreateCheckoutSessionMutation: vi.fn(),
}));

vi.mock("../../mutations/useCreateCheckoutSessionMutation", () => ({
  useCreateCheckoutSessionMutation: (...args: unknown[]) =>
    mockUseCreateCheckoutSessionMutation(...args),
}));

import { UpgradeToProButton } from "../UpgradeToProButton";

describe("UpgradeToProButton", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    Object.defineProperty(globalThis, "location", {
      value: { href: "", origin: "https://app.test" },
      writable: true,
    });
    mockUseCreateCheckoutSessionMutation.mockReturnValue({
      mutate: mockMutate,
      isPending: false,
    });
    mockMutate.mockImplementation((_request, options) => {
      options?.onSuccess?.("https://checkout.stripe.com/session");
    });
  });

  it("starts checkout and redirects on success", () => {
    render(<UpgradeToProButton priceId="price_monthly" />);

    fireEvent.click(screen.getByRole("button", { name: /Upgrade to Pro/i }));

    expect(mockMutate).toHaveBeenCalledWith(
      {
        successUrl: "https://app.test/subscription-success",
        cancelUrl: "https://app.test/settings?upgrade=cancelled",
        priceId: "price_monthly",
      },
      expect.objectContaining({
        onSuccess: expect.any(Function),
      }),
    );
    expect(globalThis.location.href).toBe("https://checkout.stripe.com/session");
  });

  it("renders custom button label", () => {
    render(
      <UpgradeToProButton priceId="price_monthly">
        Go Pro now
      </UpgradeToProButton>,
    );

    expect(screen.getByRole("button", { name: /Go Pro now/i })).toBeInTheDocument();
  });

  it("shows redirecting state while pending", () => {
    mockUseCreateCheckoutSessionMutation.mockReturnValue({
      mutate: mockMutate,
      isPending: true,
    });

    render(<UpgradeToProButton priceId="price_monthly" />);

    expect(screen.getByRole("button", { name: /Redirecting/i })).toBeDisabled();
  });
});
