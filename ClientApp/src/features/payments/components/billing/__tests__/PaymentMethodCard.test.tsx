import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { paymentMethod } from "@/features/payments/__tests__/fixtures";

vi.mock("@/features/payments/components/BillingPortalButton", () => ({
  BillingPortalButton: ({ children }: { children?: React.ReactNode }) => (
    <button type="button">{children}</button>
  ),
}));

import { PaymentMethodCard } from "../PaymentMethodCard";

describe("PaymentMethodCard", () => {
  it("renders saved payment method details", () => {
    render(<PaymentMethodCard paymentMethod={paymentMethod()} />);

    expect(screen.getByText(/VISA ending in 4242/i)).toBeInTheDocument();
    expect(screen.getByText(/Expires 03\/2028 · Jane Doe/i)).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Update Payment Method/i }),
    ).toBeInTheDocument();
  });

  it("shows expiring soon badge", () => {
    render(
      <PaymentMethodCard paymentMethod={paymentMethod({ isExpiringSoon: true })} />,
    );

    expect(screen.getByText("Expiring soon")).toBeInTheDocument();
  });

  it("handles missing expiration details", () => {
    render(
      <PaymentMethodCard
        paymentMethod={paymentMethod({ expMonth: 0, expYear: 0, billingName: undefined })}
      />,
    );

    expect(screen.getByText(/Expires —/i)).toBeInTheDocument();
  });

  it("prompts to add a payment method when none exists", () => {
    render(<PaymentMethodCard paymentMethod={undefined} />);

    expect(screen.getByText("No payment method on file")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Add Payment Method/i }),
    ).toBeInTheDocument();
  });
});
