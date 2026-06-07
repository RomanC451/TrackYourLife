import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

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

import { BillingPortalButton } from "../BillingPortalButton";

describe("BillingPortalButton", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseSuspenseQuery.mockReturnValue({
      data: "https://billing.stripe.com/portal",
      isPending: false,
    });
    Object.defineProperty(globalThis, "location", {
      value: { href: "" },
      writable: true,
    });
  });

  it("renders default label and opens the billing portal", () => {
    render(<BillingPortalButton />, { wrapper: createQueryClientWrapper() });

    fireEvent.click(screen.getByRole("button", { name: /Manage Billing/i }));

    expect(globalThis.location.href).toBe("https://billing.stripe.com/portal");
  });

  it("supports custom children and url route suffix", () => {
    render(
      <BillingPortalButton urlRoute="/payment-methods">
        Update card
      </BillingPortalButton>,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.click(screen.getByRole("button", { name: /Update card/i }));

    expect(globalThis.location.href).toBe(
      "https://billing.stripe.com/portal/payment-methods",
    );
  });
});
